namespace GREngine.Core.System;

using global::System;
using global::System.Collections;
using global::System.Collections.Generic;
using global::System.Linq;
using global::System.Reflection;
using Microsoft.Xna.Framework;

using static Debug.Out;

public sealed partial class SceneManager(Game game) : GameComponent(game), ISceneManager
{
    private readonly List<Scene> scenes = [];
    private Scene? activeScene;
    private Scene? nextScene;

    private readonly Node rootNode = new();
    private readonly Node persistentNode = new();

    private readonly SortedSet<Behaviour> activeBehaviours =
        new(new Behaviour.LoadOrderComparer());
    private readonly HashSet<Behaviour> initializationSet = [];
    private readonly HashSet<Behaviour> disposeSet = [];
    private readonly HashSet<Action<GameTime>> lateUpdateQueue = [];
    private readonly Dictionary<string, HashSet<Node>> nodeTagIndex = new();

    #region MAIN
    public override void Update(GameTime gameTime)
    {
        if (this.nextScene != null)
            this.TransitionScene();

        if (this.activeScene != null)
            this.UpdateActiveScene(gameTime);

        base.Update(gameTime);
    }

    private void InitializeBehaviours()
    {
        List<Behaviour> initializationQueue = this.initializationSet.ToList();

        // calculate load order for each behaviour [REFLECTION USED HERE]
        initializationQueue.ForEach(b =>
        {
            IEnumerable<Attribute> attrs = b.GetType().GetTypeInfo().GetCustomAttributes();

            Attribute? loadOrderAttribute = attrs.ToList().FindLast(
                a => a.GetType() == typeof(GRExecutionOrderAttribute));

            // ReSharper disable once MergeConditionalExpression
            int loadOrder = loadOrderAttribute ==
                            null ? 0 : ((GRExecutionOrderAttribute)loadOrderAttribute).LoadOrder;
            b.Initialize(loadOrder, this, this.Game);
            this.initializationSet.Remove(b);
        });

        // uses CompareTo function of behaviour, which uses load order
        initializationQueue.Sort(new Behaviour.LoadOrderComparer());

        // Awake functions
        initializationQueue.ForEach(b => b.OnAwake());

        // run start functions for enabled behaviours (in load order)
        initializationQueue.FindAll(b => b.Enabled).ForEach(b =>
        {
            b.OnStart();

            // automatically sorted to load order by data type
            this.activeBehaviours.Add(b);
        });

        // Add initialized and started scripts to regular update loop
        // activeBehaviours = Algorithms.Sort.MergeSortedLists(
        //                      this.activeBehaviours, enabledBehaviours) as List<Behaviour>
        //                    ?? throw new InvalidOperationException(
        //                                  "Active behaviour sorting resulted in a null list!");
    }

    private void UpdateActiveScene(GameTime gameTime)
    {
        // update currently enabled behaviours

        // sort initialization queue if non-empty
        // iterate the initialization queue, running initialize on each behaviour, running awake
        // take behaviours that are enabled, drop the rest
        // merge initialization queue and activeBehaviours list

        if (this.initializationSet.Count != 0)
            this.InitializeBehaviours();

        foreach (Behaviour b in this.activeBehaviours)
            b.OnUpdate(gameTime);

        if (this.lateUpdateQueue.Count > 0)
        {
            this.lateUpdateQueue.ToList().ForEach(a => a.Invoke(gameTime));
            this.lateUpdateQueue.Clear();
        }

        if (this.disposeSet.Count != 0)
        {
            foreach (Behaviour b in this.disposeSet)
                DeInitBehaviour(b);

            this.disposeSet.Clear();
            GC.Collect();
        }
    }
    #endregion

    #region CURRENT_SCENE_API
    private void BootstrapBehaviour(Behaviour b, Node node)
    {
        b.Node = new NodePointer(node);
        b.Game = Game;
        if (!b.Initialized) this.initializationSet.Add(b);
    }

    void ISceneManager.BehaviourEnabledChanged(Behaviour behaviour, bool enabled)
    { // [DONE]
        // add or remove behaviour from active list
        // if add, check if initialized, if so, add to and
        // resort active list, otherwise add to initialization list instead
        if (enabled)
        {
            if (behaviour.Initialized)
                this.activeBehaviours.Add(behaviour);
            else
                this.initializationSet.Add(behaviour);
        }
        else
        {
            this.activeBehaviours.Remove(behaviour);
        }
    }

    void ISceneManager.NodeEnabledChanged(ReadOnlySpan<Behaviour> behaviours, bool status)
    {
        // BehaviourEnabledChanged but with list
        foreach (Behaviour t in behaviours)
            ((ISceneManager)this).BehaviourEnabledChanged(t, status);
    }

    private static void DeInitBehaviour(Behaviour behaviour)
    {
        behaviour.OnDestroy();
        // this.activeBehaviours.Remove(behaviour);
#pragma warning disable // For a behaviour to be loaded at all, it must be attached to a Node
        behaviour.Node.Get().Behaviours.Remove(behaviour);
#pragma warning restore
        behaviour.Node.Dangle();
    }

    // private void DestroyGraphComponents(Node node)
    // {
    //     // depth first tree traversal
    //
    //     TraverseGraphNodes(node, n =>
    //     {
    //         n.behaviours.FindAll(b => b.Initialized).ForEach(DeInitBehaviour);
    //         return n.children;
    //     });
    // }

    private static bool IsDescendedFrom(Node node, Node parent) => GetFirstAncestor(node) == parent;

    private static Node GetFirstAncestor(Node node)
    {
        while (true)
        {
            if (node.Parent == null)
            {
                return node;
            }
            node = node.Parent;
        }
    }

    private static void TraverseGraphNodes(Node start, Func<Node, IEnumerable<Node>> function)
    {
        foreach (Node child in function(start))
        {
            TraverseGraphNodes(child, function);
        }
    }

    private void AppendTagIndex(string tag, Node node)
    {
        if (this.nodeTagIndex.TryGetValue(tag, out HashSet<Node>? value))
        {
            value.Add(node);
        }
        else
        {
            this.nodeTagIndex.Add(tag, [node]);
        }
    }

    // ReSharper disable UnusedMember.Local
    private void AppendTagIndexList(string tag, HashSet<Node> nodes)
        // ReSharper restore UnusedMember.Local
    {
        if (!this.nodeTagIndex.TryAdd(tag, nodes))
            this.nodeTagIndex[tag] = this.nodeTagIndex[tag].Concat(nodes) as HashSet<Node> ?? nodes;
    }

    private void RemoveTagIndex(string tag, Node node)
    {
#if DEBUG
        if (!this.nodeTagIndex.ContainsKey(tag))
            throw new InvalidOperationException("Trying to update indexes for non-existent tags");
#endif

        this.nodeTagIndex[tag].Remove(node);
    }
    #endregion

    #region SCENES_API
    private Scene GetSceneByName(string scene)
    {
        Scene? res = this.scenes.Find(s => s.Name == scene);

#if DEBUG
        if (res is null)
            throw new ArgumentException("No such scene with that name");
#endif

        return res;
    }

    private void UnloadCurrentScene()
    {
        // this.UnloadChildren(this.rootGameObject);
        // // this should unload all the monogame assets from the previous scene
        // this.activeScene.OnUnload();
        //
        // // this should delete the entire scene graph from the previous scene
        // this.rootGameObject.ClearChildren();

        // unload the entire scene graph from node Root
        // delete the scene graph from node Root
        // unload scene content manager

        // user defined behaviour de-init
        // DestroyGraphComponents(this.rootNode);

        // user defined unload
#pragma warning disable
        this.activeScene.OnUnload();
#pragma warning restore

        // wipe ephemeral tree
        this.rootNode.Children.Clear();

        // wipe scene data
        this.activeBehaviours.RemoveWhere(b =>
        {
#pragma warning disable
            if (!IsDescendedFrom(b.Node.Get(), this.rootNode)) return false;
#pragma warning restore

            DeInitBehaviour(b);
            return true;
        });

        this.initializationSet.Clear();
        this.nodeTagIndex.Clear();

        // unload assets
        this.activeScene.ContentManager.Unload();
        this.activeScene.ContentManager.Dispose();
    }

    private void TransitionScene()
    {
        if (this.activeScene != null)
        {
            // maybe only GC.Collect() if there's more than a certain amount of objects in the scene

            UnloadCurrentScene();

            //  Perform a garbage collection to ensure memory is cleared
            GC.Collect();
        }

        this.activeScene = this.nextScene;
        this.nextScene = null;

        // guaranteed to be not null by ChangeScene function not having a nullable (?) parameter
        this.activeScene?.OnLoad(this);
    }
    #endregion

    #region DEBUG
    private static void PrintChildren(Node node, int depth) => PrintChildren(new NodePointer(node), depth);

    private static void PrintChildren(NodePointer node, int depth)
    {
        // depth first tree traversal
        ++depth;

        string space = "";
        for (int i = 0; i < depth; i++)
            space += "   ";

        string components = "";
        node.GetAllBehaviours().ToList().ForEach(c => { components += c.GetType().Name + ":" + c.Name + ", "; });

        string tags = "";
        node.Get().Tags.ToList().ForEach(c => { tags += c + ","; });

        Vector3 position = node.GetLocalPosition();
        const string format = "{0,10:####0.000}";
        PrintLn(
            "[" + string.Format(format, position.X) + ", " +
            string.Format(format, position.Y) + ", " + string.Format(format, position.Z) + "]" +
            space +
            node.GetType().Name + ": '" + node.Get().Name + "' -> [" + components + "]" + " <" + tags + ">"
        );

        IEnumerable<NodePointer> g = node.GetChildren();
        foreach (NodePointer child in g.ToList())
            PrintChildren(child, depth);
    }
    #endregion
}

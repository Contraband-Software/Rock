namespace GREngine.Core.System;

using Debug;
using global::System;
using global::System.Collections.Generic;
using global::System.IO;
using global::System.Linq;
using global::System.Reflection;
using global::System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using static GREngine.Debug.Out;

public sealed class SceneManager : GameComponent, ISceneControllerService
{
    private readonly List<Scene> scenes = new();
    private Scene? activeScene;
    private Scene? nextScene;

    private readonly RootNode rootNode = new();
    private readonly RootNode persistantNode = new();

    private SortedSet<Behaviour> activeBehaviours = new SortedSet<Behaviour>();
    private HashSet<Behaviour> initializationSet = new HashSet<Behaviour>();
    private Dictionary<string, HashSet<Node>> nodeTagIndex = new Dictionary<string, HashSet<Node>>();

    private HashSet<Action<GameTime>> lateUpdateQueue = new HashSet<Action<GameTime>>();

    public SceneManager(Game game) : base(game)
    {
    }

    public override void Initialize()
    {
        base.Initialize();
    }

    // private class LoadOrderComparison : IComparer<Behaviour>
    // {
    //     public int Compare(Behaviour a, Behaviour b)
    //     {
    //         return a.loadOrder.CompareTo(b.loadOrder);
    //     }
    // }

    #region MONOGAME
    public override void Update(GameTime gameTime)
    {
        if (this.nextScene != null)
        {
            this.TransitionScene();
        }

        if(this.activeScene != null)
        {
            // update currently enabled behaviours

            // sort initialization queue if non-empty
                // iterate the initialization queue, running initialize on each behaviour, running awake
                // take behaviours that are enabled, drop the rest
                // merge initialization queue and activeBehaviours list

            foreach (Behaviour b in this.activeBehaviours)
            {
                b.OnUpdate(gameTime);
            }

            if (this.initializationSet.Count != 0)
            {
                List<Behaviour> initializationQueue = this.initializationSet.ToList();
                // calculate load order for each behaviour [REFLECTION USED HERE]
                initializationQueue.ForEach(b =>
                {
                    IEnumerable<Attribute> attrs = b.GetType().GetTypeInfo().GetCustomAttributes();

                    Attribute? loadOrderAttribute = attrs.ToList().FindLast(a => a.GetType() == typeof(GRExecutionOrderAttribute));

                    int loadOrder = loadOrderAttribute == null ? 0 : ((GRExecutionOrderAttribute)loadOrderAttribute).LoadOrder;
                    b.Initialize(loadOrder, this, this.Game);
                });

                // uses CompareTo function of behaviour, which uses load order
                initializationQueue.Sort();

                // Awake functions (in load order)
                initializationQueue.ForEach(b => b.OnAwake());

                // run start functions for enabled behaviours (in load order)
                List<Behaviour> enabledBehaviours = initializationQueue.FindAll(b => b.Enabled);
                enabledBehaviours.ForEach(b => b.OnStart());

                // Add initialized and started scripts to regular update loop (in load order)
                // activeBehaviours = Algorithms.Sort.MergeSortedLists(this.activeBehaviours, enabledBehaviours) as List<Behaviour>
                //                    ?? throw new InvalidOperationException("Active behaviour sorting resulted in a null list!");

                enabledBehaviours.ForEach(b => this.activeBehaviours.Add(b));
                this.initializationSet.Clear();

                GC.Collect();
            }

            if (this.lateUpdateQueue.Count > 0)
            {
                this.lateUpdateQueue.ToList().ForEach(a => a.Invoke(gameTime));
                this.lateUpdateQueue.Clear();
            }
        }

        base.Update(gameTime);
    }
    #endregion

    #region CURRENT_SCENE_API
    public void QueueSceneAction(Action<GameTime> action)
    {
        this.lateUpdateQueue.Add(action);
    }

    public Node? FindNodeWithTag(string tag)
    {
        #if DEBUG
        if (!this.nodeTagIndex.ContainsKey(tag))
        {
            throw new ArgumentOutOfRangeException("Tag does not exist: " + tag);
        }
        #endif

        if (this.nodeTagIndex[tag].Count == 0)
        {
            return null;
        }
        return nodeTagIndex[tag].First();
    }

    public HashSet<Node> FindNodesWithTag(string tag)
    {
        #if DEBUG
        if (!this.nodeTagIndex.ContainsKey(tag))
        {
            throw new ArgumentOutOfRangeException("Tag does not exist: " + tag);
        }
        #endif

        return nodeTagIndex[tag];
    }

    public void AddNodeAtPersistent(Node node)
    {
        AddNode(this.persistantNode, node);
    }

    public void AddNodeAtRoot(Node node)
    {
        AddNode(this.rootNode, node);
    }

    /// <summary>
    /// If any node behaviours are disabled, they are not initialized
    /// </summary>
    /// <param name="node"></param>
    /// <param name="parent"></param>
    public void AddNode(Node parent, Node node)
    {
            // add node to parent's child list
            // set node parent to parent

            // get TagList from object, add values to tagindex
            // find attribute node tags, add values to tagindex, and add values to object TagList

            // add node's components to initialization queue, adding the Node parent ref

        // repeat this process for child nodes

        // sort the initialization queue

        parent.children.Add(node);
        node.parent = parent;

        IEnumerable<Attribute> attrs = node.GetType().GetTypeInfo().GetCustomAttributes();
        Attribute? tagsAttribute = attrs.ToList().FindLast(a => a.GetType() == typeof(GRETagWithAttribute));
        if (tagsAttribute != null)
        {
            ((GRETagWithAttribute)tagsAttribute).Tags.ToList().ForEach(t => node.Tags.Add(t));
        }

        node.Tags.ToList().ForEach(t => AppendTagIndex(t, node));

        node.behaviours.ForEach(b =>
        {
            b.Node = node;
            if (!b.Initialized) this.initializationSet.Add(b);
        });
    }

    public void DestroyNode(Node node)
    {
        // throw new NotImplementedException();

        // get node TagList, remove node reference from tag indexes
        // get all component instances, run their OnDestroy, remove them from activeBehaviours
        // repeat for all children
        // delete node and sub tree
        // garbage collect

        node.parent?.children.Remove(node);
        node.parent = null;

        TraverseGraphNodes(node,
            n =>
            {
                n.Tags.ToList().ForEach(t => RemoveTagIndex(t, node));
                n.behaviours.FindAll(b => b.Initialized).ForEach(this.DeInitBehaviour);
                return n.children;
            });

        GC.Collect();
    }

    public RootNode GetRootNode()
    {
        return this.rootNode;
    }

    public RootNode GetPersistentNode()
    {
        return this.persistantNode;
    }

    /// <summary>
    /// Do not add a behaviour instance that has already been added, it will fuck shit up
    /// </summary>
    /// <param name="node"></param>
    /// <param name="behaviour"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void AddBehaviour(Node node, Behaviour behaviour)
    {
        // always assumed to be an uninitialized behaviour
        // add reference to node's behaviour list
        // if enabled, add behaviour to initialization list

        behaviour.Node = node;
        node.behaviours.Add(behaviour);
        if (!behaviour.Initialized) this.initializationSet.Add(behaviour);
    }
    public Behaviour InitBehaviour(Node node, Behaviour behaviour)
    {
        this.AddBehaviour(node, behaviour);
        return behaviour;
    }
    public void RemoveBehaviour(Behaviour behaviour)
    {
        // run on destroy
        // remove behaviour from active list
        // remove behaviour from node list

        DeInitBehaviour(behaviour);
        behaviour.Node.behaviours.Remove(behaviour);
        behaviour.Node = null;
    }
    public void RemoveBehavioursWithTag(Node node, string tag)
    {
        // RemoveBehaviour but with tag

        foreach (Behaviour b in node.behaviours.Where(behaviour => behaviour.Tags.Contains(tag)))
        {
            RemoveBehaviour(b);
        }
    }

    void ISceneControllerService.BehaviourEnabledChanged(Behaviour behaviour, bool status)
    { // [DONE]
        // add or remove behaviour from active list
        // if add, check if initialized, if so, add to and resort active list, otherwise add to initialization list instead
        if (status)
        {
            if (behaviour.Initialized)
            {
                this.activeBehaviours.Add(behaviour);
            }
            else
            {
                this.initializationSet.Add(behaviour);
            }
        }
        else
        {
            this.activeBehaviours.Remove(behaviour);
        }
    }

    void ISceneControllerService.NodeEnabledChanged(ReadOnlySpan<Behaviour> behaviours, bool status)
    {
        // BehaviourEnabledChanged but with list
        for (int i = 0; i < behaviours.Length; i++)
        {
            ((ISceneControllerService)this).BehaviourEnabledChanged(behaviours[i], status);
        }
    }

    private void DeInitBehaviour(Behaviour behaviour)
    {
        behaviour.OnDestroy();
        this.activeBehaviours.Remove(behaviour);
    }

    private void DestroyGraphComponents(Node node)
    {
        // depth first tree traversal

        TraverseGraphNodes(node, n =>
        {
            n.behaviours.FindAll(b => b.Initialized).ForEach(this.DeInitBehaviour);
            return n.children;
        });
    }

    private static bool IsDescendedFromRoot(Node node, RootNode parent)
    {
        return GetFirstAncestor(node) == parent;
    }

    private static Node GetFirstAncestor(Node node)
    {
        while (true)
        {
            if (node.parent == null)
            {
                return node;
            }
            node = node.parent;
        }
    }

    private void TraverseGraphNodes(Node start, Func<Node, IEnumerable<Node>> function)
    {
        foreach (Node child in function(start))
        {
            this.TraverseGraphNodes(child, function);
        }
    }

    private void AppendTagIndex(string tag, Node node)
    {
        if (this.nodeTagIndex.ContainsKey(tag))
        {
            this.nodeTagIndex[tag].Add(node);
        }
        else
        {
            this.nodeTagIndex.Add(tag, new HashSet<Node> { node });
        }
    }

    private void AppendTagIndexList(string tag, HashSet<Node> nodes)
    {
        if (!this.nodeTagIndex.TryAdd(tag, nodes))
        {
            this.nodeTagIndex[tag] = this.nodeTagIndex[tag].Concat(nodes) as HashSet<Node> ?? nodes;
        }
    }

    private void RemoveTagIndex(string tag, Node node)
    {
        #if DEBUG
        if (!this.nodeTagIndex.ContainsKey(tag))
        {
            throw new InvalidOperationException("Trying to update indexes for non-existent tags");
        }
        #endif

        this.nodeTagIndex[tag].Remove(node);
    }
    #endregion

    #region SCENE_API
    public Scene? GetCurrentScene()
    {
        return this.activeScene;
    }

    /// <summary>
    /// Registers a scene. Must be used before trying to load said scene.
    /// </summary>
    /// <param name="scene"></param>
    public void AddScene(Scene scene)
    {
        scene.Initialize(Game);
        this.scenes.Add(scene);
    }

    public void ChangeScene(string next)
    {
        Scene nextScene = this.GetSceneByName(next);

        if(this.activeScene != nextScene)
        {
            this.nextScene = nextScene;
        }
    }

    public void ReloadCurrentScene()
    {
        this.nextScene = this.activeScene;
    }

    private Scene GetSceneByName(string scene)
    {
        foreach (Scene s in this.scenes)
        {
            if (s.Name == scene)
            {
                return s;
            }
        }

        throw new ArgumentException("No such scene with that name");
    }

    private void TransitionScene()
    {
        if(this.activeScene != null)
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
            DestroyGraphComponents(this.rootNode);

            // user defined unload
            this.activeScene.OnUnload();

            // wipe ephemeral tree
            this.rootNode.children.Clear();

            // wipe scene data
            this.activeBehaviours.RemoveWhere(b =>
            {
                if (!IsDescendedFromRoot(b.Node, this.rootNode)) return false;

                this.DeInitBehaviour(b);
                return true;
            });

            this.initializationSet.Clear();
            this.nodeTagIndex.Clear();

            // unload assets
            this.activeScene.contentManager.Unload();
            this.activeScene.contentManager.Dispose();
        }

        //  Perform a garbage collection to ensure memory is cleared
        GC.Collect();

        this.activeScene = this.nextScene;

        this.nextScene = null;

        // guaranteed to be not null by ChangeScene function not having a nullable (?) parameter
        this.activeScene?.OnLoad(this);
    }
    #endregion

    #region DEBUG
    private const int POSITION_PADDING = 32;

    /// <summary>
    /// Prints a text-version of the scene tree (has indentation for children)
    /// </summary>
    public void DebugPrintGraph()
    {
        string rootName = "<SceneController>";
        PrintLn(rootName.PadLeft(POSITION_PADDING + rootName.Length + "    ".Length));
        this.PrintChildren(this.rootNode, 0);
        this.PrintChildren(this.persistantNode, 0);
    }

    private void PrintChildren(Node node, int depth)
    {
        // depth first tree traversal
        ++depth;

        string space = "";
        for (int i = 0; i < depth; i++)
        {
            space += "   ";
        }

        string components = "";
        node.GetAllBehaviours().ToList().ForEach(c => { components += c.GetType().Name + ":" + c.Name + ", "; });

        string tags = "";
        node.Tags.ToList().ForEach(c => { tags += c + ","; });

        Vector3 position = node.GetGlobalPosition();
        string format = "{0,10:####0.000}";
        PrintLn(
            "[" + String.Format(format, position.X) + ", " + String.Format(format, position.Y) + ", " + String.Format(format, position.Z) + "]" +
            space +
            node.GetType().Name + ": '" + node.Name + "' -> [" + components + "]" + " <" + tags + ">"
        );

        IEnumerable<Node> g = node.GetChildren();
        foreach (Node child in g.ToList())
        {
            this.PrintChildren(child, depth);
        }
    }
    #endregion
}

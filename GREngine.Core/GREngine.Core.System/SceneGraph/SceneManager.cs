using NotImplementedException = System.NotImplementedException;

namespace GREngine.Core.System;

using global::System;
using global::System.Collections.Generic;
using global::System.Linq;
using global::System.Reflection;
using Microsoft.Xna.Framework;

using static GREngine.Debug.Out;

public sealed partial class SceneManager : GameComponent, ISceneControllerService
{
    private readonly List<Scene> scenes = new();
    private Scene? activeScene;
    private Scene? nextScene;

    private readonly RootNode rootNode = new();
    private readonly RootNode persistantNode = new();

    private SortedSet<Behaviour> activeBehaviours = new SortedSet<Behaviour>();
    private List<Behaviour> initializationQueue = new List<Behaviour>();
    private Dictionary<string, List<Node>> nodeTagIndex = new Dictionary<string, List<Node>>();

    public SceneManager(Game game) : base(game)
    {

    }

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

            if (this.initializationQueue.Count != 0)
            {
                // calculate load order for each behaviour [REFLECTION USED HERE]
                this.initializationQueue.ForEach(b =>
                {
                    TypeInfo typeInfo = gameTime.GetType().GetTypeInfo();
                    IEnumerable<Attribute> attrs = typeInfo.GetCustomAttributes();

                    Attribute? loadOrderAttribute = attrs.ToList().FindLast(a => a.GetType() == typeof(GRExecutionOrderAttribute));

                    int loadOrder = loadOrderAttribute == null ? 0 : ((GRExecutionOrderAttribute)loadOrderAttribute).LoadOrder;
                    b.Initialize(loadOrder, this, this.Game);
                });

                // uses CompareTo function of behaviour, which uses load order
                this.initializationQueue.Sort();

                // Awake functions (in load order)
                this.initializationQueue.ForEach(b => b.OnAwake());

                // run start functions for enabled behaviours (in load order)
                List<Behaviour> enabledBehaviours = this.initializationQueue.FindAll(b => b.Enabled);
                enabledBehaviours.ForEach(b => b.OnStart());

                // Add initialized and started scripts to regular update loop (in load order)
                // activeBehaviours = Algorithms.Sort.MergeSortedLists(this.activeBehaviours, enabledBehaviours) as List<Behaviour>
                //                    ?? throw new InvalidOperationException("Active behaviour sorting resulted in a null list!");

                enabledBehaviours.ForEach(b => this.activeBehaviours.Add(b));
                this.initializationQueue.Clear();

                GC.Collect();
            }
        }

        base.Update(gameTime);
    }
    #endregion

    #region CURRENT_SCENE_API
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

    public List<Node> FindNodesWithTag(string tag)
    {
        #if DEBUG
        if (!this.nodeTagIndex.ContainsKey(tag))
        {
            throw new ArgumentOutOfRangeException("Tag does not exist: " + tag);
        }
        #endif

        return nodeTagIndex[tag];
    }

    public void AddNode(Node node, Node parent)
    {
            // add node to parent's child list
            // set node parent to parent

            // get TagList from object, add values to tagindex
            // find attribute node tags, add values to tagindex, and add values to object TagList

            // add node's components to initialization queue, adding the Node parent ref

        // repeat this process for child nodes

        // sort the initialization queue
    }

    public void DestroyNode(Node node)
    {
        throw new NotImplementedException();

        // get node TagList, remove node reference from tag indexes
        // get all component instances, run their OnDestroy, remove them from activeBehaviours
        // repeat for all children
        // delete node and sub tree
        // garbage collect
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
        if (behaviour.Enabled) this.initializationQueue.Add(behaviour);
    }
    public void RemoveBehaviour(Behaviour behaviour)
    {
        // run on destroy
        // remove behaviour from active list
        // remove behaviour from node list

        behaviour.OnDestroy();
        behaviour.Node?.behaviours.Remove(behaviour);
        behaviour.Node = null;
        this.activeBehaviours.Remove(behaviour);
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
                this.initializationQueue.Add(behaviour);
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

    private void DestroyGraphComponents(Node node)
    {
        // depth first tree traversal

        node.behaviours.FindAll(b => b.Initialized).ForEach(b => b.OnDestroy());
        foreach (Node child in node.GetChildren())
        {
            this.DestroyGraphComponents(child);
        }
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
            this.activeBehaviours.Clear();
            this.initializationQueue.Clear();
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
        this.activeScene?.OnLoad(this.rootNode, this.persistantNode);
    }
    #endregion

    #region DEBUG
    private const int POSITION_PADDING = 32;

    /// <summary>
    /// Prints a text-version of the scene tree (has indentation for children)
    /// </summary>
    public void DebugPrintGraph()
    {
        string rootName = "SceneController";
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
        Vector3 position = node.GetGlobalPosition();
        string format = "{0,10:####0.000}";
        PrintLn(
            "[" + String.Format(format, position.X) + ", " + String.Format(format, position.Y) + ", " + String.Format(format, position.Z) + "]" +
            space +
            node.GetType().Name + ": '" + node.Name + "' -> [" + components + "]"
        );

        IEnumerable<Node> g = node.GetChildren();
        foreach (Node child in g.ToList())
        {
            this.PrintChildren(child, depth);
        }
    }
    #endregion
}

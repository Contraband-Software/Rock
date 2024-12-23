namespace GREngine.Core.System;

using global::System;
using global::System.Collections.Generic;
using global::System.Linq;
using global::System.Reflection;
using Microsoft.Xna.Framework;

using static Debug.Out;

public sealed partial class SceneManager
{
    #region DEBUG
    private const int POSITION_PADDING = 32;

    /// <summary>
    /// Prints a text-version of the scene tree (has indentation for children)
    /// </summary>
    public void DebugPrintGraph()
    {
        const string rootName = "<SceneController>";
        PrintLn(rootName.PadLeft(POSITION_PADDING + rootName.Length + "    ".Length));
        PrintChildren(this.rootNode, 0);
        PrintChildren(this.persistentNode, 0);
    }
    #endregion

    #region SCENES_API
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
        Scene scene = this.GetSceneByName(next);

        if (this.activeScene != scene)
        {
            this.nextScene = scene;
        }
    }

    public void ReloadCurrentScene()
    {
        this.nextScene = this.activeScene;
    }
    #endregion


    #region CURRENT_SCENE_API
    #region NODE_GRAPH

    public NodePointer GetRootNode()
    {
        return new NodePointer(this.rootNode);
    }

    public NodePointer GetPersistentNode()
    {
        return new NodePointer(this.persistentNode);
    }

    public NodePointer AddNodeAtRoot(string name)
    {
        var x = new Node(name);
        if (name == "Player")
            x.Tags.Add("Player");
        return LoadNode(this.rootNode, x);
    }

    public NodePointer AddNodeAtPersistent(string name)
    {
        return LoadNode(this.persistentNode, new Node(name));
    }

    /// <summary>
    /// If any node behaviours are disabled, they are not initialized
    /// </summary>
    /// <param name="node"></param>
    /// <param name="parent"></param>
    public NodePointer AddNode(NodePointer parentNode, string name)
    {
        // add node to parent's child list
        // set node parent to parent

        // get TagList from object, add values to the tag index
        // find attribute node tags, add values to the tag index, and add values to object TagList

        // add node's components to initialization queue, adding the Node parent ref

        // repeat this process for child nodes

        // sort the initialization queue
#if DEBUG
        if (parentNode.Get() == null)
            throw new ArgumentException("Null parent Node pointer passed into AddNode");
#endif
        return LoadNode(parentNode.Get()!, new Node(name));
    }

    public NodePointer AddNode(string name)
    {
        // add node to parent's child list
        // set node parent to parent

        // get TagList from object, add values to the tag index
        // find attribute node tags, add values to the tag index, and add values to object TagList

        // add node's components to initialization queue, adding the Node parent ref

        // repeat this process for child nodes

        // sort the initialization queue
        return LoadNode(this.rootNode, new Node(name));
    }

    // node tag caches need to be cleared when a weak ref to a node is done
    // also nodes should have weak refs to each other
    private NodePointer LoadNode(Node parent, Node node)
    {
        node.parent = new NodePointer(parent);
        parent.children.Add(node);
        node.sceneManager = this;

        IEnumerable<Attribute> attrs = node.GetType().GetTypeInfo().GetCustomAttributes();
        Attribute? tagsAttribute = attrs.ToList().FindLast(a => a.GetType() == typeof(GRETagWithAttribute));
        if (tagsAttribute != null)
            ((GRETagWithAttribute)tagsAttribute).Tags.ToList().ForEach(t => node.Tags.Add(t));

        node.Tags.ToList().ForEach(t => AppendTagIndex(t, node));

        node.behaviours.ForEach(b =>
        {
            BootstrapBehaviour(b, node);
        });

        return new NodePointer(node);
    }

    public void DestroyNode(NodePointer node)
    {
        node.Free();
    }

    internal void FreeNode(Node node)
    {
        // will run the same code if the node ref has already been unloaded
        // throw new NotImplementedException();

        // get node TagList, remove node reference from tag indexes
        // get all component instances, run their OnDestroy, remove them from activeBehaviours
        // repeat for all children
        // delete node and sub tree
        // garbage collect

        node.parent.Get()!.children.Remove(node);
        node.parent = null!;

        TraverseGraphNodes(node,
            n =>
            {
                // PrintLn(n.Tags.ToList().Count.ToString());
                n.Tags.ToList().ForEach(t => RemoveTagIndex(t, node));
                n.behaviours.FindAll(b => b.Initialized).ForEach(b => this.disposeSet.Add(b));
                n.behaviours.ForEach(b => this.initializationSet.Remove(b));
                return n.children;
            });
    }

    public NodePointer? FindNodeWithTag(string tag)
    {
#if DEBUG
        if (!this.nodeTagIndex.ContainsKey(tag))
        {
            throw new ArgumentOutOfRangeException(tag, "Tag does not exist");
        }
#endif

        return new NodePointer(
                (this.nodeTagIndex[tag].Count == 0
                    ? null : this.nodeTagIndex[tag].First()
                )!
            );
    }

    public HashSet<NodePointer> FindNodesWithTag(string tag)
    {
#if DEBUG
        if (!this.nodeTagIndex.ContainsKey(tag))
        {
            throw new ArgumentOutOfRangeException("Tag does not exist: " + tag);
        }
#endif

        return nodeTagIndex[tag].Select(n => new NodePointer(n)).ToHashSet();
    }
    #endregion

    #region BEHAVIOUR_SCRIPTING
    public void QueueSceneAction(Action<GameTime> action)
    {
        this.lateUpdateQueue.Add(action);
    }

    /// <summary>
    /// Do not add a behaviour instance that has already been added, it will seriously mess everything up
    /// </summary>
    /// <param name="node"></param>
    /// <param name="behaviour"></param>
    /// <exception cref="NotImplementedException"></exception>
    public Behaviour AddBehaviour(NodePointer node, Behaviour behaviour)
    {
        // always assumed to be an uninitialized behaviour
        // add reference to node's behaviour list
        // if enabled, add behaviour to initialization list
#if DEBUG
        if (node.Get()!.behaviours.Contains(behaviour))
        {
            throw new ArgumentException("This Behaviour has already been added to this Node");
        }
#endif

        node.Get()!.behaviours.Add(behaviour);
        BootstrapBehaviour(behaviour, node.Get()!);
        return behaviour;
    }

    public void RemoveBehaviour(Behaviour behaviour)
    {
        // run on destroy
        // remove behaviour from active list
        // remove behaviour from node list

        this.initializationSet.Remove(behaviour);
        this.disposeSet.Add(behaviour);
    }

    public void RemoveBehavioursWithTag(NodePointer node, string tag)
    {
        // RemoveBehaviour but with tag

        foreach (Behaviour b in node.Get()!.behaviours.Where(behaviour => behaviour.Tags.Contains(tag)))
        {
            RemoveBehaviour(b);
        }
    }
    #endregion
    #endregion
}

namespace GREngine.Core.System;

using global::System;
using global::System.Collections.Generic;

public interface ISceneControllerService
{
    public void DebugPrintGraph();

    public RootNode GetRootNode();
    public RootNode GetPersistentNode();

    public void QueueSceneAction(Action action);
    public void AddBehaviour(Node node, Behaviour behaviour);
    public void RemoveBehaviour(Behaviour behaviour);
    public void RemoveBehavioursWithTag(Node node, string tag);
    public void AddNode(Node node, Node parent);
    public void DestroyNode(Node node);

    public Node? FindNodeWithTag(string tag);
    public HashSet<Node> FindNodesWithTag(string tag);
    public void AddNodeAtPersistent(Node node);
    public void AddNodeAtRoot(Node node);

    public Scene? GetCurrentScene();
    public void AddScene(Scene scene);
    public void ChangeScene(string next);
    public void ReloadCurrentScene();

    internal void BehaviourEnabledChanged(Behaviour behaviour, bool status);
    internal void NodeEnabledChanged(ReadOnlySpan<Behaviour> behaviours, bool status);
}

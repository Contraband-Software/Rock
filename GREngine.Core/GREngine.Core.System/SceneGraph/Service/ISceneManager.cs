namespace GREngine.Core.System;

using global::System;
using global::System.Collections.Generic;
using Microsoft.Xna.Framework;

public interface ISceneManager
{
    // ReSharper disable UnusedMemberInSuper.Global UnusedMember.Global
    public void DebugPrintGraph();

    public NodePointer GetRootNode();
    public NodePointer GetPersistentNode();

    public void QueueSceneAction(Action<GameTime> action);
    public Behaviour AddBehaviour(NodePointer node, Behaviour behaviour);
    public void RemoveBehaviour(Behaviour behaviour);
    public void RemoveBehavioursWithTag(NodePointer node, string tag);
    public NodePointer AddNode(NodePointer parent, string name);
    public void DestroyNode(NodePointer node);

    public NodePointer? FindNodeWithTag(string tag);
    public HashSet<NodePointer> FindNodesWithTag(string tag);
    public NodePointer AddNodeAtPersistent(string name);
    public NodePointer AddNodeAtRoot(string name);

    public Scene? GetCurrentScene();
    public void AddScene(Scene scene);
    public void ChangeScene(string next);
    public void ReloadCurrentScene();

    internal void BehaviourEnabledChanged(Behaviour behaviour, bool status);
    internal void NodeEnabledChanged(ReadOnlySpan<Behaviour> behaviours, bool status);
    // ReSharper restore UnusedMemberInSuper.Global UnusedMember.Global
}

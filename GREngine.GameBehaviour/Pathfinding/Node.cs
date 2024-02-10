namespace GREngine.GameBehaviour.Pathfinding;

using Microsoft.Xna.Framework;

internal class Node
{
    internal enum NodeState { Untested, Open, Closed }

    // LocalPosition
    public Point Location { get; private set; }
    public bool IsWalkable { get; set; } = true;
    public Node? ParentNode { get; set; }

    internal float G { get; private set; }
    internal float H { get; private set; }
    internal float F { get { return this.G + this.H; } }
    internal NodeState State { get; set; } = NodeState.Untested;
}

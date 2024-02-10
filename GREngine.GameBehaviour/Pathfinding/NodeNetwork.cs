namespace GREngine.GameBehaviour.Pathfinding;

using System;
using System.Collections.Generic;
using System.Linq;
using Core.System;
using Microsoft.Xna.Framework;

public class NodeNetwork : Behaviour
{
    private Node startNode;
    private Node endNode;

    private int width;
    private int height;
    private Node[,] nodeNetwork;

    public NodeNetwork()
    {

    }

    protected override void OnAwake()
    {
        // this.Node.GetBehaviour<>()
    }

    private bool Search(Node currentNode)
    {
        currentNode.State = Pathfinding.Node.NodeState.Closed;
        List<Node> nextNodes = GetAdjacentWalkableNodes(currentNode);
        nextNodes.Sort((node1, node2) => node1.F.CompareTo(node2.F));
        foreach (var nextNode in nextNodes)
        {
            if (nextNode.Location == this.endNode.Location)
                return true;
            if (this.Search(nextNode)) // Note: Recurses back into Search(Node)
                return true;
        }
        return false;
    }

    private List<Point> FindPath()
    {
        List<Point> path = new List<Point>();
        bool success = Search(startNode);
        if (success)
        {
            Node node = this.endNode;
            while (node.ParentNode != null)
            {
                path.Add(node.Location);
                node = node.ParentNode;
            }
            path.Reverse();
        }
        return path;
    }

    private List<Node> GetAdjacentWalkableNodes(Node fromNode)
    {
        List<Node> walkableNodes = new List<Node>();
        IEnumerable<Point> nextLocations = GetAdjacentLocations(fromNode.Location);

        foreach (var location in nextLocations)
        {
            int x = location.X;
            int y = location.Y;

            // Stay within the grid's boundaries
            if (x < 0 || x >= this.width || y < 0 || y >= this.height)
                continue;

            Node node = this.nodeNetwork[x, y];
            // Ignore non-walkable nodes
            if (!node.IsWalkable)
                continue;

            // Ignore already-closed nodes
            if (node.State == Pathfinding.Node.NodeState.Closed)
                continue;

            // Already-open nodes are only added to the list if their G-value is lower going via this route.
            if (node.State == Pathfinding.Node.NodeState.Open)
            {
                float traversalCost = GetTraversalCost(node.Location, node.ParentNode.Location);
                float gTemp = fromNode.G + traversalCost;
                if (gTemp < node.G)
                {
                    node.ParentNode = fromNode;
                    walkableNodes.Add(node);
                }
            }
            else
            {
                // If it's untested, set the parent and flag it as 'Open' for consideration
                node.ParentNode = fromNode;
                node.State = Pathfinding.Node.NodeState.Open;
                walkableNodes.Add(node);
            }
        }

        return walkableNodes;
    }

    private float GetTraversalCost(Point p1, Point p2)
    {

        return MathF.Sqrt(MathF.Pow(p1.X - p2.X, 2) + MathF.Pow(p1.Y - p2.Y, 2));
    }

    private IEnumerable<Point> GetAdjacentLocations(Point p)
    {
        List<Point> adj = new();
        for (int y = 0; y < this.nodeNetwork.Length; y++)
        {
            for (int x = 0; x < this.nodeNetwork.Length; x++)
            {
                if (this.nodeNetwork[y, x].Location.Equals(p))
                {
                    int l = this.nodeNetwork.Length;

                    int y_1 = Math.Clamp(y - 1, 0, l);
                    adj.Add(this.nodeNetwork[y_1, Math.Clamp(x - 1, 0, l)].Location);
                    adj.Add(this.nodeNetwork[y_1, x].Location);
                    adj.Add(this.nodeNetwork[y_1, Math.Clamp(x + 1, 0, l)].Location);

                    int y_a1 = Math.Clamp(y + 1, 0, l);
                    adj.Add(this.nodeNetwork[y_a1, Math.Clamp(x - 1, 0, l)].Location);
                    adj.Add(this.nodeNetwork[y_a1, x].Location);
                    adj.Add(this.nodeNetwork[y_a1, Math.Clamp(x + 1, 0, l)].Location);

                    adj.Add(this.nodeNetwork[y, Math.Clamp(x - 1, 0, l)].Location);
                    adj.Add(this.nodeNetwork[y, Math.Clamp(x + 1, 0, l)].Location);

                    return adj;
                }
            }
        }
        throw new InvalidOperationException("Pathfinding error: no node with position: " + p);
    }
}

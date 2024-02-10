namespace GREngine.GameBehaviour.Pathfinding;

using System;
using System.Collections.Generic;
using System.Linq;
using Core.System;
using Debug;
using Microsoft.Xna.Framework;

using GREngine.Core.PebbleRenderer;

public class NodeNetwork : Behaviour
{
    private Node startNode;
    private Node endNode;

    private int width;
    private int height;
    private Node[,] nodeNetwork = null;

    public NodeNetwork()
    {

    }

    protected override void OnAwake()
    {
        // this.Node.GetBehaviour<>()
    }

    public void BuildNetwork(int width, int height, string mapCollisionLayer, string wallCollisionLayer, float resolution)
    {
        this.width = width;
        this.height = height;

        Vector2 position = new Vector2();
        this.Node.GetLocalPosition().Deconstruct(out position.X, out position.Y, out _);

        int networkRight = (int)MathF.Floor(position.Y + width);
        int networkBottom = (int)MathF.Floor(position.X + height);

        this.nodeNetwork = new Node[
            width  / ((int)(width  / resolution)),
            height / ((int)(height / resolution))];

        int vx = 0;
        int vy = 0;

        for (int y = (int)position.Y; y < networkBottom; y += (int)(height / resolution))
        {
            for (int x = (int)position.X; x < networkRight; x += (int)(width / resolution))
            {
                Out.PrintLn(vx.ToString() + ", " + vy);
                this.nodeNetwork[vy, vx] = new Node(new Point(x, y), true);
                vx = (vx + 1) % ((int)resolution);
            }
            vy++;
        }

        // Out.PrintLn(vx.ToString() + ", " + vy);
    }

    protected override void OnUpdate(GameTime gameTime)
    {
        if (this.nodeNetwork != null)
        {
            foreach (var node in this.nodeNetwork)
            {
                this.Game.Services.GetService<IPebbleRendererService>().drawDebug(new DebugDrawable(node.Location.ToVector2(), 8, Color.Aqua));
            }
        }
        if (this.startNode != null)
            this.Game.Services.GetService<IPebbleRendererService>().drawDebug(new DebugDrawable(this.startNode.Location.ToVector2(), 10, Color.Lime));
        if (this.endNode != null)
            this.Game.Services.GetService<IPebbleRendererService>().drawDebug(new DebugDrawable(this.endNode.Location.ToVector2(), 10, Color.Gold));
    }

    public List<Point> Navigate(Point start, Point end)
    {
        this.startNode = GetNearestNode(start);
        this.endNode = GetNearestNode(end);

        foreach (Node node in this.nodeNetwork)
        {
            node.InitCosts(start, end);
        }

        #if DEBUG
        if (this.nodeNetwork == null)
        {
            throw new InvalidOperationException("Pathfinding error: trying to navigate when network has not been built!");
        }
        #endif

        return FindPath();
    }

    private Node GetNearestNode(Point position)
    {
        float shortestDistance = float.PositiveInfinity;
        Node closestNode = null;
        foreach (var node in this.nodeNetwork)
        {
            float dist = GetTraversalCost(position, node.Location);
            if (dist < shortestDistance)
            {
                shortestDistance = dist;
                closestNode = node;
            }
        }
        return closestNode;
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

            Node node = GetNearestNode(location);
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

    internal static float GetTraversalCost(Point p1, Point p2)
    {
        return MathF.Sqrt(MathF.Pow(p1.X - p2.X, 2) + MathF.Pow(p1.Y - p2.Y, 2));
    }

    private IEnumerable<Point> GetAdjacentLocations(Point p)
    {
        List<Point> adj = new();
        for (int y = 0; y < this.nodeNetwork.Length; y++)
        {
            for (int x = 0; x < this.nodeNetwork.GetLength(1); x++)
            {
                // Out.PrintLn(x + ", " + y);
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

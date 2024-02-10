namespace GREngine.GameBehaviour.Pathfinding;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Core.Physics2D;
using Core.System;
using Debug;
using Microsoft.Xna.Framework;

using GREngine.Core.PebbleRenderer;
using Color = Microsoft.Xna.Framework.Color;
using Point = Microsoft.Xna.Framework.Point;

public class NodeNetwork : Behaviour
{
    private Node startNode;
    private Node endNode;

    private int width;
    private int height;
    private Node[,] nodeNetwork = null;

    public List<Point> LastPath { get; private set; }

    public NodeNetwork()
    {

    }

    protected override void OnAwake()
    {
        // this.Node.GetBehaviour<>()
    }

    public void BuildNetwork(int width, int height, string makePointLayer, string cullPointLayer, float resolution)
    {
        ICollisionSystem col = this.Game.Services.GetService<ICollisionSystem>();

        // Out.PrintLn((col == null).ToString());

        this.width = width;
        this.height = height;

        Vector2 position = new Vector2();
        this.Node.GetLocalPosition().Deconstruct(out position.X, out position.Y, out _);

        int networkRight = (int)MathF.Floor(position.Y + width);
        int networkBottom = (int)MathF.Floor(position.X + height);

        this.nodeNetwork = new Node[
            width  / ((int)MathF.Ceiling(width  / resolution)),
            height / ((int)MathF.Ceiling(height / resolution))];

        int vx = 0;
        int vy = 0;

        // Out.PrintLn(this.nodeNetwork.GetLength(0).ToString());
        // Out.PrintLn(this.nodeNetwork.GetLength(1).ToString());

        for (int y = (int)position.Y;; y += (int)(height / resolution))
        {
            if (vy == this.nodeNetwork.GetLength(0))
                break;
            vx = 0;
            for (int x = (int)position.X;; x += (int)(width / resolution))
            {
                if (vx == this.nodeNetwork.GetLength(1))
                    break;
                // Out.PrintLn(vx.ToString() + ", " + vy);

                Vector2 globalPosition = new Vector2();
                (Node.GetGlobalPosition() + new Vector3(new PointF(x, y).ToVector2(), 0)).Deconstruct(out globalPosition.X, out globalPosition.Y, out _);

                // Out.PrintLn("collision");
                // Out.PrintLn(globalPosition.ToString());
                // Out.PrintLn(col.PointIsCollidingWithLayer(new PointF(globalPosition.X, globalPosition.Y), makePointLayer).ToString());
                bool dogshit = col.PointIsCollidingWithLayer(new PointF(globalPosition.X, globalPosition.Y), makePointLayer);
                this.nodeNetwork[vy, vx] = new Node(
                    new Point(x, y),
                    dogshit
                    // && !col.PointIsCollidingWithLayer(new PointF(x, y), cullPointLayer)
                    );
                vx++;

            }
            vy++;
        }
    }

    protected override void OnUpdate(GameTime gameTime)
    {
        if (this.nodeNetwork != null)
        {
            foreach (var node in this.nodeNetwork)
            {
                if (node.IsWalkable)
                    this.Game.Services.GetService<IPebbleRendererService>().drawDebug(new DebugDrawable(node.Location.ToVector2(), 6, Color.Aqua));
            }
        }
        if (this.startNode != null)
            this.Game.Services.GetService<IPebbleRendererService>().drawDebug(new DebugDrawable(this.startNode.Location.ToVector2(), 10, Color.Lime));
        if (this.endNode != null)
            this.Game.Services.GetService<IPebbleRendererService>().drawDebug(new DebugDrawable(this.endNode.Location.ToVector2(), 10, Color.Gold));
        if (LastPath != null)
        {
            Point lastPoint = this.startNode.Location;
            foreach (var p in LastPath)
            {
                this.Game.Services.GetService<IPebbleRendererService>().drawDebug(new DebugDrawable(lastPoint.ToVector2(), p.ToVector2(), Color.Crimson, DebugShape.LINE));
                lastPoint = p;
            }
        }
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
        LastPath = path;
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

                    if (y - 1 >= 0)
                    {
                        if (x - 1 >= 0) adj.Add(this.nodeNetwork[y - 1, x - 1].Location);
                        adj.Add(this.nodeNetwork[y - 1, x].Location);
                        if (x + 1 < l) adj.Add(this.nodeNetwork[y - 1, x + 1].Location);
                    }

                    if (y + 1 < l)
                    {
                        if (x - 1 >= 0) adj.Add(this.nodeNetwork[y + 1, x - 1].Location);
                        adj.Add(this.nodeNetwork[y + 1, x].Location);
                        if (x + 1 < l) adj.Add(this.nodeNetwork[y + 1, x + 1].Location);
                    }

                    if (x - 1 >= 0) adj.Add(this.nodeNetwork[y, x - 1].Location);
                    if (x + 1 < l) adj.Add(this.nodeNetwork[y, x  + 1].Location);

                    return adj;
                }
            }
        }
        throw new InvalidOperationException("Pathfinding error: no node with position: " + p);
    }
}

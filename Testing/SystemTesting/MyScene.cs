namespace Testing.SystemTesting;

using System.Drawing;
using GREngine.Core.Physics2D;
using GREngine.Core.System;
using GREngine.Debug;
using GREngine.GameBehaviour.Pathfinding;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Point = Microsoft.Xna.Framework.Point;

public class MyScene : Scene
{
    public MyScene() : base("MyScene")
    {
    }

    protected override void OnLoad(Node rootNode, Node persistantNode)
    {
        MyNode myNode = new MyNode();
        MyBehaviour myBehaviour = new MyBehaviour();

        ISceneControllerService sc = this.Game.Services.GetService<ISceneControllerService>();

        sc.AddBehaviour(myNode, myBehaviour);
        sc.AddBehaviour(myNode, new UpdateNotifier());
        sc.AddNodeAtRoot(myNode);

        GenericNode node = new GenericNode();
        NodeNetwork network = new NodeNetwork();
        sc.AddBehaviour(node, network);
        sc.AddNode(myNode, node);

        GenericNode map = new GenericNode();
        map.SetLocalPosition(1, 1);
        CircleCollider mapCol1 = new CircleCollider(100, "mapFloor", true);
        sc.AddBehaviour(map, mapCol1);
        sc.AddNodeAtRoot(map);

        sc.QueueSceneAction((GameTime gt) =>
        {
            node.SetLocalPosition(new Vector2(-50, -50));
            network.BuildNetwork(600, 600, "mapFloor", "", 10);

            network.Navigate(new Point(10, 10), new Point(300, 300));
            Out.PrintLn(mapCol1.PointInsideCollider(new PointF(300, 300)).ToString());

            foreach (var p in network.LastPath)
            {
                Out.PrintLn(p.ToString());
            }
        });
    }
}

class UpdateNotifier : Behaviour
{
    protected override void OnUpdate(GameTime gameTime)
    {
        // this.Game.Services.GetService<ISceneControllerService>().DebugPrintGraph();
    }
}

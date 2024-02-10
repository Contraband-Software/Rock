namespace Testing.SystemTesting;

using System.Collections.Generic;
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

        GenericNode map = new GenericNode();
        NodeNetwork nodeNetwork = sc.InitBehaviour(map, new NodeNetwork()) as NodeNetwork;
        sc.AddBehaviour(map, new CircleCollider(100, "mapFloor", true));
        sc.AddNodeAtRoot(map);

        GenericNode map_col1 = new GenericNode();
        sc.AddBehaviour(map_col1, new CircleCollider(100, "mapFloor", true));
        sc.AddNode(map, map_col1);

        map.SetLocalPosition(100, 100);

        sc.QueueSceneAction((GameTime gt) =>
        {
            nodeNetwork.BuildNetwork(600, 600, "mapFloor", "", 20);

            List<Point> path = nodeNetwork.Navigate(new Point(10, 10), new Point(300, 300));

            if (path != null)
            {
                foreach (var p in path)
                {
                    Out.PrintLn(p.ToString());
                }
            }

            sc.DebugPrintGraph();
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

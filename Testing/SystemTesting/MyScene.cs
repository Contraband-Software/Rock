namespace Testing.SystemTesting;

using System.Collections.Generic;
using System.Drawing;
using GREngine.Core.PebbleRenderer;
using GREngine.Core.Physics2D;
using GREngine.Core.System;
using GREngine.Debug;
using GREngine.GameBehaviour.Pathfinding;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;
using Point = Microsoft.Xna.Framework.Point;

public class MyScene : Scene
{
    public MyScene() : base("MyScene")
    {
    }

    protected override void OnLoad(SceneManager sceneManager)
    {
        MyNode myNode = new MyNode();
        MyBehaviour myBehaviour = new MyBehaviour();

        ISceneControllerService sc = this.Game.Services.GetService<ISceneControllerService>();

        sc.AddBehaviour(myNode, myBehaviour);
        sc.AddBehaviour(myNode, new UpdateNotifier());
        sc.AddNodeAtRoot(myNode);

        GenericNode map = new GenericNode("map parent");
        NodeNetwork nodeNetwork = sc.InitBehaviour(map, new NodeNetwork()) as NodeNetwork;
        Collider c0 = sc.InitBehaviour(map, new CircleCollider(230, "mapFloor", true)) as Collider;
        c0.SetStatic(true);
        sc.AddNodeAtRoot(map);

        GenericNode map_col1 = new GenericNode();
        Collider c1 = sc.InitBehaviour(map_col1, new CircleCollider(100, "mapFloor", true)) as Collider;
        c1.SetStatic(true);
        sc.AddNode(map, map_col1);
        map_col1.SetLocalPosition(300, 300);

        GenericNode map_col2 = new GenericNode();
        Collider c2 = sc.InitBehaviour(map_col2, new CircleCollider(100, "mapFloor", true)) as Collider;
        c2.SetStatic(true);
        sc.AddNode(map, map_col2);
        map_col2.SetLocalPosition(300, 160);

        GenericNode map_col3 = new GenericNode();
        Collider c3 = sc.InitBehaviour(map_col3, new CircleCollider(100, "mapWall", true)) as Collider;
        c3.SetStatic(true);
        sc.AddNode(map, map_col3);
        map_col3.SetLocalPosition(100, 160);

        map.SetLocalPosition(300, 100);

        sc.QueueSceneAction((GameTime gt) =>
        {
            nodeNetwork.BuildNetwork(600, 600, "mapFloor", "mapWall", 20);

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

        sc.QueueSceneAction(_ =>
        {
            Out.PrintLn("#########");
            Out.PrintLn(map.GetGlobalPosition().ToString());
            Out.PrintLn(map_col1.GetLocalPosition().ToString());
            Out.PrintLn(map_col1.GetGlobalPosition().ToString());
        });
    }
}

class UpdateNotifier : Behaviour
{
    protected override void OnUpdate(GameTime gameTime)
    {
        // this.Game.Services.GetService<ISceneControllerService>().DebugPrintGraph();
        this.Game.Services.GetService<IPebbleRendererService>().drawDebug(new DebugDrawable(new Vector2(300, 100), 4, Color.HotPink));
        this.Game.Services.GetService<IPebbleRendererService>().drawDebug(new DebugDrawable(new Vector2(300, 300), 8, Color.HotPink));
        this.Game.Services.GetService<IPebbleRendererService>().drawDebug(new DebugDrawable(new Vector2(600, 400), 14, Color.HotPink));
    }
}

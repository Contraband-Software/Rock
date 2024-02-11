namespace Testing.SystemTesting;

using GREngine.Core.Physics2D;
using GREngine.Core.System;
using GREngine.Debug;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Drawing;
using Testing.CollisionTesting;

public class GeneralCollisionTesting : Scene
{
    Texture2D texture;
    Texture2D pixelTexture;
    public GeneralCollisionTesting() : base("GeneralCollisionTesting")
    {
    }

    protected override void OnLoad(SceneManager sceneManager)
    {
        MyNode myNode = new MyNode();
        MyBehaviour myBehaviour = new MyBehaviour();

        this.Game.Services.GetService<ISceneControllerService>().AddBehaviour(myNode, myBehaviour);
        this.Game.Services.GetService<ISceneControllerService>().AddNodeAtRoot(myNode);

        ICollisionSystem collisionSystem = this.Game.Services.GetService<ICollisionSystem>();

        List<PointF> squarePointFList = new List<PointF>
        {
            new PointF(-20, -20),
            new PointF(20, -20),
            new PointF(20, 20),
            new PointF(-20, 20)
        };

        //Collider object 1
        GenericNode node1 = new GenericNode();

        node1.SetLocalPosition(new Vector2(100f, 100f));
        sceneManager.AddNodeAtRoot(node1);

        PolygonCollider col1 = new PolygonCollider(squarePointFList, true);
        sceneManager.AddBehaviour(node1, col1);

        //Circle collider 1
        GenericNode node2 = new GenericNode();
        node2.SetLocalPosition(new Vector2(100f, 160f));
        sceneManager.AddNodeAtRoot(node2);
        CircleCollider circ1 = new CircleCollider(20f);
        sceneManager.AddBehaviour(node2, circ1);

        List<string> rayLayers = new List<string> {
            "default"
        };

        sceneManager.QueueSceneAction((_) =>
        {
            col1.SetRotation(45f);
            Raycast2DResult ray = collisionSystem.Raycast2D(new PointF(100f, 300f), new Vector2(0f, -1f), 1000f, rayLayers);
            //Out.PrintLn(ray.collisionNormal.ToString());
        });
    }
}

public class ColTest : Behaviour
{
    protected override void OnUpdate(GameTime time)
    {
        Out.PrintLn("Position: " + this.Node.GetLocalPosition().ToString());
    }
}

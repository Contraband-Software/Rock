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
/*        GenericNode node1 = new GenericNode();

        node1.SetLocalPosition(new Vector2(100f, 100f));
        sceneManager.AddNodeAtRoot(node1);

        PolygonCollider col1 = new PolygonCollider(squarePointFList, true);
        sceneManager.AddBehaviour(node1, col1);*/

        //Circle collider 1
        GenericNode node2 = new GenericNode();
        node2.SetLocalPosition(new Vector2(100f, 100f));
        sceneManager.AddNodeAtRoot(node2);
        CircleCollider circ1 = new CircleCollider(20f);
        sceneManager.AddBehaviour(node2, circ1);

        //Circle collider 2
        GenericNode node3 = new GenericNode();
        node3.SetLocalPosition(new Vector2(200f, 200f));
        sceneManager.AddNodeAtRoot(node3);
        CircleCollider circ2 = new CircleCollider(30f);
        sceneManager.AddBehaviour(node3, circ2);

        circ2.SetAllowedCollisionLayers(new List<string> { "default"});

        List<string> rayLayers = new List<string> {
            "default"
        };

        sceneManager.QueueSceneAction((_) =>
        {
            circ2.SetVelocity(new Vector2(-1f, -1f));
            //col1.SetRotation(45f);
            Raycast2DResult ray = collisionSystem.Raycast2D(new PointF(50f, 50f), new Vector2(1f, 1f), 1000f, rayLayers);
            Out.PrintLn(ray.collisionNormal.ToString());
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

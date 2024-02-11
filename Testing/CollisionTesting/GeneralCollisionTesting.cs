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
        /*GenericNode node1 = new GenericNode();

        node1.SetLocalPosition(new Vector2(100f, 100f));
        sceneManager.AddNodeAtRoot(node1);

        PolygonCollider col1 = new PolygonCollider(squarePointFList, new Vector2(0f, 100f), true);
        sceneManager.AddBehaviour(node1, col1);

        //Collider object 2
        GenericNode node2 = new GenericNode();

        node2.SetLocalPosition(new Vector2(200f, 90f));
        sceneManager.AddNodeAtRoot(node2);

        PolygonCollider col2 = new PolygonCollider(squarePointFList, new Vector2(0f, 100f), true);
        sceneManager.AddBehaviour(node2, col2);*/

        //Collider object 3
        GenericNode node3 = new GenericNode();

        node3.SetLocalPosition(new Vector2(80f, 400f));
        sceneManager.AddNodeAtRoot(node3);

        PolygonCollider col3 = new PolygonCollider(squarePointFList, true);
        sceneManager.AddBehaviour(node3, col3);

        //Circle collider object 1
        GenericNode node4 = new GenericNode();
        node4.SetLocalPosition(new Vector2(100,300));
        sceneManager.AddNodeAtRoot(node4);
        CircleCollider circCol1 = new CircleCollider(15f, new Vector2(0f, 100f), true);
        circCol1.SetLayer("map");
        sceneManager.AddBehaviour(node4, circCol1);

        /*//Circle collider object 2
        GenericNode node5 = new GenericNode();
        node5.SetLocalPosition(new Vector2(600, 300));
        sceneManager.AddNodeAtRoot(node5);
        CircleCollider circCol2 = new CircleCollider(30f, new Vector2(0f, 100f), true);
        circCol1.SetLayer("map");
        sceneManager.AddBehaviour(node5, circCol2);

        // Circle collider object 3
        GenericNode node6 = new GenericNode();
        node6.SetLocalPosition(new Vector2(600, 400));
        sceneManager.AddNodeAtRoot(node6);
        CircleCollider circCol3 = new CircleCollider(15f, new Vector2(0f, 100f), true);
        circCol1.SetLayer("map");
        sceneManager.AddBehaviour(node6, circCol3);*/

        sceneManager.QueueSceneAction((_) =>
        {
            //col1.SetRotation(45f);
            //col3.SetRotation(45);
            //col1.SetVelocity(new Vector2(0.2f, 0f));
            //col2.SetVelocity(new Vector2(-0.2f, 0f));
            col3.SetRotation(45f);
            col3.SetVelocity(new Vector2(0f, -0.2f));
            //circCol2.SetTrigger(true);
            //circCol3.SetVelocity(new Vector2(0f, -0.2f));

            collisionSystem.PointIsCollidingWithLayer(new PointF(130, 130), "map");
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

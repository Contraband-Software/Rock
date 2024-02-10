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

    protected override void OnLoad(Node rootNode, Node persistantNode)
    {
        MyNode myNode = new MyNode();
        MyBehaviour myBehaviour = new MyBehaviour();

        this.Game.Services.GetService<ISceneControllerService>().AddBehaviour(myNode, myBehaviour);
        this.Game.Services.GetService<ISceneControllerService>().AddNodeAtRoot(myNode);

        ISceneControllerService sceneManager = this.Game.Services.GetService<ISceneControllerService>();
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

        PolygonCollider col1 = new PolygonCollider(squarePointFList);
        collisionSystem.AddCollisionObject(col1);
        sceneManager.AddBehaviour(node1, col1);

        //Collider object 2
        GenericNode node2 = new GenericNode();

        node2.SetLocalPosition(new Vector2(200f, 100f));
        sceneManager.AddNodeAtRoot(node2);

        PolygonCollider col2 = new PolygonCollider(squarePointFList);
        collisionSystem.AddCollisionObject(col2);
        sceneManager.AddBehaviour(node2, col2);

        //Circle collider object 1
        GenericNode node3 = new GenericNode();
        node3.SetLocalPosition(new Vector2(60,0));
        sceneManager.AddNodeAtRoot(node3);
        CircleCollider circCol1 = new CircleCollider(50f);
        sceneManager.AddBehaviour(node3, circCol1);

        sceneManager.QueueSceneAction(() =>
        {
            col1.SetRotation(45f);
            col1.SetVelocity(new Vector2(0.2f, 0f));
            col2.SetVelocity(new Vector2(-0.2f, 0f));
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

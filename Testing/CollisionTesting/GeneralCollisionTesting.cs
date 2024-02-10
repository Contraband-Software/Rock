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

        GenericNode node1 = new GenericNode();
        sceneManager.AddBehaviour(node1, new ColTest());

        node1.SetLocalPosition(new Vector2(100f, 100f));
        sceneManager.AddNodeAtRoot(node1);

        PolygonCollider col1 = new PolygonCollider(squarePointFList);
        collisionSystem.AddCollisionObject(col1);
        sceneManager.AddBehaviour(node1, col1);

        sceneManager.AddBehaviour(node1, new ColliderVelocityController(col1));
    }
}

public class ColTest : Behaviour
{
    protected override void OnUpdate(GameTime time)
    {
        Out.PrintLn("Position: " + this.Node.GetLocalPosition().ToString());
    }
}

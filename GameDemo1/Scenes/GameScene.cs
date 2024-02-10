namespace GameDemo1.Scenes;

using GREngine.Core.Physics2D;
using GREngine.Core.System;
using GREngine.Debug;
using Microsoft.Xna.Framework;
using Scripts;

public class GameScene : Scene
{
    private ISceneControllerService sceneController;
    private const string mapFloorCollisionLayer = "mapFloor";

    public GameScene() : base("GameScene")
    {
    }

    private Node MakeLevelBlock(CircleCollider col, Vector2 position)
    {
        GenericNode mapNode = new GenericNode();
        col.SetTrigger(true);
        col.SetLayer(mapFloorCollisionLayer);
        sceneController.AddBehaviour(mapNode, col);
        mapNode.SetLocalPosition(position);
        return mapNode;
    }

    protected override void OnLoad(SceneManager sceneManager)
    {


        sceneController = sceneManager;

        Player n1 = new Player();
        sceneManager.AddNodeAtRoot(n1);
        n1.SetLocalPosition(100, 100);
        CircleCollider cc = sceneManager.InitBehaviour(n1, new CircleCollider(20, true)) as CircleCollider;
        sceneManager.AddBehaviour(n1, new PlayerController(mapFloorCollisionLayer));

        GenericNode mapRoot = new GenericNode();
        sceneManager.AddNodeAtRoot(mapRoot);
        this.sceneController.AddNode(mapRoot, MakeLevelBlock(new CircleCollider(400, true), new Vector2(100, 101)));
        this.sceneController.AddNode(mapRoot, MakeLevelBlock(new CircleCollider(240, true), new Vector2(-600, -600)));
        this.sceneController.AddNode(mapRoot, MakeLevelBlock(new CircleCollider(200, true), new Vector2(80, 700)));
        this.sceneController.AddNode(mapRoot, MakeLevelBlock(new CircleCollider(200, true), new Vector2(1000, -70)));

        sceneManager.QueueSceneAction(_ =>
        {
            sceneManager.DebugPrintGraph();
        });
    }
}

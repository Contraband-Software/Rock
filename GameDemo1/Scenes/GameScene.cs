

namespace GameDemo1.Scenes;

using GREngine.Core.Physics2D;
using GREngine.Core.System;
using GREngine.GameBehaviour.Pathfinding;
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
        GenericNode mapNode = new GenericNode("MapGround");
        col.SetTrigger(true);
        col.SetLayer(mapFloorCollisionLayer);
        sceneController.AddBehaviour(mapNode, col);
        mapNode.SetLocalPosition(position);
        return mapNode;
    }

    protected override void OnLoad(SceneManager sceneManager)
    {
        sceneController = sceneManager;

        #region MANAGERS
        Player n1 = new Player();
        sceneManager.AddNodeAtRoot(n1);
        n1.SetLocalPosition(100, 100);
        CircleCollider cc = sceneManager.InitBehaviour(n1, new CircleCollider(20, true)) as CircleCollider;
        PlayerController pc = new PlayerController(mapFloorCollisionLayer);
        sceneManager.AddBehaviour(n1, pc);

        GenericNode gameManager = new GenericNode("GameManager");
        GameController gc = new GameController();
        sceneManager.AddBehaviour(gameManager, gc);
        sceneManager.AddNodeAtRoot(gameManager);
        #endregion

        #region MAP
        GenericNode mapRoot = new GenericNode("MapRoot");
        sceneManager.AddNodeAtRoot(mapRoot);


        GenericNode pathNode = new GenericNode("PathFindingNetwork");
        pathNode.SetLocalPosition(-1000, -1000);
        NodeNetwork pathfindingNetwork = sceneManager.InitBehaviour(pathNode, new NodeNetwork()) as NodeNetwork;
        sceneManager.AddNodeAtRoot(pathNode);

        this.sceneController.AddNode(mapRoot, MakeLevelBlock(new CircleCollider(400, true), new Vector2(100, 101)));
        this.sceneController.AddNode(mapRoot, MakeLevelBlock(new CircleCollider(240, true), new Vector2(-600, -700)));
        this.sceneController.AddNode(mapRoot, MakeLevelBlock(new CircleCollider(200, true), new Vector2(80, 700)));
        this.sceneController.AddNode(mapRoot, MakeLevelBlock(new CircleCollider(200, true), new Vector2(1000, -170)));
        this.sceneController.AddNode(mapRoot, MakeLevelBlock(new CircleCollider(150, true), new Vector2(-900, 570)));

        sceneManager.QueueSceneAction(_ =>
        {
            pathfindingNetwork.BuildNetwork(3000, 3000, mapFloorCollisionLayer, "", 30);
        });
        #endregion


        sceneManager.QueueSceneAction(_ =>
        {
            sceneManager.DebugPrintGraph();
        });
    }
}

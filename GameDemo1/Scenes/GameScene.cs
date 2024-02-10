namespace GameDemo1.Scenes;

using GREngine.Core.Physics2D;
using GREngine.Core.System;
using GREngine.Debug;
using Scripts;

public class GameScene : Scene
{
    public GameScene() : base("GameScene")
    {
    }

    protected override void OnLoad(SceneManager sceneManager)
    {
        string mapFloorCollisionLayer = "mapFloor";

        Player n1 = new Player();
        sceneManager.AddNodeAtRoot(n1);
        n1.SetLocalPosition(100, 100);
        sceneManager.AddBehaviour(n1, new CircleCollider(20, mapFloorCollisionLayer, true));
        sceneManager.AddBehaviour(n1, new PlayerController(mapFloorCollisionLayer));

        GenericNode mapNode = new GenericNode();
        sceneManager.AddNodeAtRoot(mapNode);
        CircleCollider c1 = new CircleCollider(400, true);
        c1.SetTrigger(true);
        sceneManager.AddBehaviour(mapNode, c1);
    }
}

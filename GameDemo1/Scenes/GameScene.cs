namespace GameDemo1.Scenes;

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
        Player n1 = new Player();
        sceneManager.AddNodeAtRoot(n1);
        sceneManager.AddBehaviour(n1, new PlayerController());

        sceneManager.QueueSceneAction(_ =>
        {
            Out.PrintLn(sceneManager.FindNodeWithTag("Player").Name);
        });
    }
}

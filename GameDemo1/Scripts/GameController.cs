namespace GameDemo1.Scripts;

using GREngine.Core.System;

[GRExecutionOrder(20)]
public class GameController : Behaviour
{
    private ISceneControllerService sc;

    private PlayerController pc;

    protected override void OnStart()
    {
        sc = this.Game.Services.GetService<ISceneControllerService>();
        InitGame();
    }

    private void InitGame()
    {
        var player = this.sc.FindNodeWithTag("Player");
        pc = (PlayerController)(player.GetBehaviour<PlayerController>());
        pc.PlayerDiedEvent += GameOver;
    }

    private void GameOver()
    {
        this.sc.DestroyNode(this.pc.Node);
    }
}

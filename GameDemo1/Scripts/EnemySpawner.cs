namespace GameDemo1.Scripts;

using GREngine.Core.PebbleRenderer;
using GREngine.Core.System;
using Microsoft.Xna.Framework;

[GRExecutionOrder(20), GRETagWith("MobSpawner")]
public class EnemySpawner : Behaviour
{
    #region SETTINGS
    private static bool debug = true;

    private static uint maxEnemies = 1000;
    #endregion

    #region STATE
    private IPebbleRendererService render;

    private float radius;
    private bool playerTouchingFrame = false;
    #endregion

    public EnemySpawner(float radius)
    {
        this.radius = radius;
    }

    public void PlayerFrameTouch()
    {
        playerTouchingFrame = true;
    }

    protected override void OnStart()
    {
        render = this.Game.Services.GetService<IPebbleRendererService>();
    }

    protected override void OnUpdate(GameTime gameTime)
    {


        if (debug) render.drawDebug(new DebugDrawable(this.Node.GetLocalPosition2D(), radius, playerTouchingFrame ? Color.Orange : Color.Aqua));

        playerTouchingFrame = false;
    }
}

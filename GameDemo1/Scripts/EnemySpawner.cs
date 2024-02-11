namespace GameDemo1.Scripts;

using System;
using GREngine.Algorithms;
using GREngine.Core.PebbleRenderer;
using GREngine.Core.System;
using Microsoft.Xna.Framework;

[GRExecutionOrder(20), GRETagWith("MobSpawner")]
public class EnemySpawner : Behaviour
{
    #region SETTINGS
    private static bool debug = true;

    private static uint maxEnemies = 10;
    #endregion

    #region STATE
    private IPebbleRendererService render;
    private ISceneControllerService sceneManager;

    public float Radius { get; private set; }
    public bool PlayerTouchingFrame { get; private set; } = false;
    #endregion

    public EnemySpawner(float radius)
    {
        this.Radius = radius;
    }

    public void PlayerFrameTouch()
    {
        PlayerTouchingFrame = true;
    }

    protected override void OnStart()
    {
        render = this.Game.Services.GetService<IPebbleRendererService>();
        sceneManager = this.Game.Services.GetService<ISceneControllerService>();


        this.SpawnRandomEnemy();
    }

    protected override void OnUpdate(GameTime gameTime)
    {
        if (debug) render.drawDebug(new DebugDrawable(this.Node.GetLocalPosition2D(), this.Radius, PlayerTouchingFrame ? Color.Orange : Color.Aqua));

        Random r = new Random();

        // if (PlayerTouchingFrame)
        // {
        //     if ((gameTime.TotalGameTime.Milliseconds) % 700 == 0)
        //     {
        //         // PrintLn(gameTime.TotalGameTime.Seconds.ToString());
        //         if (r.NextSingle() > 0.5)
        //             this.SpawnRandomEnemy();
        //     }
        // }

        // if (gameTime.TotalGameTime.Milliseconds % 200 == 0)
        // {
        //     if (r.NextSingle() > 0.5)
        //         this.SpawnRandomEnemy();
        // }


        PlayerTouchingFrame = false;
    }

    private void SpawnRandomEnemy()
    {
        Random rand = new Random();
        float angle = (float)rand.NextDouble() * MathF.Tau - MathF.PI;
        Vector2 position = Vector.AngleToVector(angle) * (this.Radius - 30);

        Enemy e = new Enemy(this);
        GenericNode enemyNode = new GenericNode("EnemyNode");
        enemyNode.SetLocalPosition(position);
        sceneManager.AddNode(this.Node, enemyNode);
        sceneManager.AddBehaviour(enemyNode, e);
    }
}

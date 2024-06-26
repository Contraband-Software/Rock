namespace GameDemo1.Scripts;

using System;
using System.Linq;
using GREngine.Algorithms;
using GREngine.Core.PebbleRenderer;
using GREngine.Core.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

[GRExecutionOrder(20), GRETagWith("MobSpawner")]
public class EnemySpawner : Behaviour
{
    #region SETTINGS
    private static bool debug = true;

    private uint maxEnemies = 4;
    #endregion

    #region STATE
    private IPebbleRendererService render;
    private ISceneControllerService sceneManager;

    public float Radius { get; private set; }
    public bool PlayerTouchingFrame { get; private set; } = false;
    #endregion

    public EnemySpawner(float radius, uint enemies)
    {
        this.Radius = radius;
        this.maxEnemies = enemies;
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
        //if (debug) render.drawDebug(new DebugDrawable(this.Node.GetLocalPosition2D(),
        // this.Radius, PlayerTouchingFrame ? Color.Orange : Color.Aqua));

        Random r = new Random();

        if (this.Node.GetChildren().ToList().Count < maxEnemies)
        {
            if (gameTime.TotalGameTime.Milliseconds % 200 == 0)
            {
                if (r.NextSingle() > 0.5)
                    this.SpawnRandomEnemy();
            }
        }

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

        Sprite enemyrender = new AnimatedSprite(0, new Vector2(0.25f),
                                                Game.Content.Load<Texture2D>("Graphics/EnemyDiffuseSS"),
                                                null, null, 3, 0, true, true);
        sceneManager.AddBehaviour(enemyNode, enemyrender);

        Light enemyLight = new Light(new Vector3(500000, 150000, 150000), false, (float)rand.NextDouble() * 4, 0.87f);
        sceneManager.AddBehaviour(enemyNode, enemyLight);
    }
}

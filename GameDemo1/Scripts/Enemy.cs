namespace GameDemo1.Scripts;

using System;
using System.Collections.Generic;
using GREngine.Algorithms;
using GREngine.Core.PebbleRenderer;
using GREngine.Core.Physics2D;
using GREngine.Core.System;
using GREngine.GameBehaviour.Pathfinding;
using Microsoft.Xna.Framework;
using Scenes;

public class Enemy : Behaviour
{
    #region SETTINGS
    private float diveDistance = 2;
    private float walkSpeed = 3;
    private float hitStrength = 70;
    #endregion

    #region STATE
    private EnemySpawner spawner;
    private PlayerController player;
    private CircleCollider collider;

    private bool onPlatform;

    private int randomMovement = 0;
    #endregion

    public Enemy(EnemySpawner spawner)
    {
        this.spawner = spawner;

        Random rand = new Random();

        this.randomMovement = (int)(rand.NextSingle() * 500);
    }

    protected override void OnStart()
    {
        ISceneControllerService sc = this.Game.Services.GetService<ISceneControllerService>();
        this.player = sc
            .FindNodeWithTag("Player")!
            .GetBehaviour<PlayerController>() as PlayerController;

        collider = this.Game.Services.GetService<ISceneControllerService>().InitBehaviour(this.Node, new CircleCollider(50, true)) as CircleCollider;
        this.collider.SetTrigger(true);
        this.collider.SetLayer(GameScene.enemyCollisionLayer);
        this.collider.SetAllowedCollisionLayers(new List<string>(){GameScene.mapFloorCollisionLayer, GameScene.enemyCollisionLayer, GameScene.mapWallCollisionLayer, GameScene.playerCollisionLayer});

        this.collider.OnTriggerEnter += with =>
        {
            if (with.GetLayer() == GameScene.playerCollisionLayer)
            {
                (this.player.Node.GetBehaviour<CircleCollider>() as CircleCollider).SetVelocity(GetPlayerDirection() * hitStrength);
            }
            if (with.GetLayer() == GameScene.mapFloorCollisionLayer)
            {
                onPlatform = true;
            }
        };
    }

    private Vector2 GetPlayerDirection()
    {
        Vector2 pos = this.Node.GetGlobalPosition2D();
        Vector2 playerPos = this.player.Node.GetGlobalPosition2D();
        return Vector.SafeNormalize(playerPos - pos);
    }

    protected override void OnUpdate(GameTime gameTime)
    {
        if (this.spawner.PlayerTouchingFrame)
        {
            this.collider.SetVelocity(GetPlayerDirection() * this.walkSpeed);
        }
        else
        {
            this.collider.SetVelocity(Vector2.Zero);
            if ((this.randomMovement + gameTime.TotalGameTime.Seconds) % (this.spawner.Radius / 20) == 0)
            {
                // this.collider.SetVelocity(-Node.GetLocalPosition2D() / 20);
            }
        }
    }
}

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

[GRExecutionOrder(8)]
public class Enemy : Behaviour
{
    #region SETTINGS
    private float diveDistance = 80;
    private float walkSpeed = 0.2f;
    private float hitStrength = 70;
    #endregion

    #region STATE
    private EnemySpawner spawner;
    private PlayerController player;
    private CircleCollider collider;

    private int randomMovement = 0;
    private int isGrounded = 0;
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

        // PrintLn(Node.GetLocalPosition2D().ToString());

        collider = this.Game.Services.GetService<ISceneControllerService>().InitBehaviour(this.Node, new CircleCollider(50, true)) as CircleCollider;
        this.collider.SetStatic(true);
        this.collider.SetTrigger(true);
        this.collider.SetLayer(GameScene.enemyCollisionLayer);
        this.collider.SetAllowedCollisionLayers(new List<string>(){GameScene.mapFloorCollisionLayer, GameScene.enemyCollisionLayer, GameScene.mapWallCollisionLayer, GameScene.playerCollisionLayer});

        this.collider.OnTriggerEnter += with =>
        {
            if (with.GetLayer() == GameScene.mapFloorCollisionLayer)
            {
                this.isGrounded = 0;
            }
            if (with.GetLayer() == GameScene.playerCollisionLayer)
            {
                Node playerNode = this.player.Node;
                if (playerNode != null)
                    ((CircleCollider)playerNode.GetBehaviour<CircleCollider>()).SetVelocity(Vector.SafeNormalize(this.GetPlayerDirection()) * this.hitStrength);
            }
        };
    }

    private Vector2 GetPlayerDirection()
    {
        Vector2 pos = this.Node.GetGlobalPosition2D();
        Vector2 playerPos = this.player.Node.GetGlobalPosition2D();
        return playerPos - pos;
    }

    private Vector2 velocity = Vector2.Zero;
    private bool lastValue;
    private bool ping = false;
    protected override void OnUpdate(GameTime gameTime)
    {
        float delta = (float)gameTime.TotalGameTime.TotalSeconds;
        this.velocity = Vector2.Zero;

        Random rand = new Random();

        if (this.Node.GetLocalPosition2D().Length() > this.spawner.Radius)
        {
            // this.Game.Services.GetService<ISceneControllerService>().DestroyNode(this.Node);
        }
        else
        {
            if (this.spawner.PlayerTouchingFrame)
            {
                if (this.spawner.PlayerTouchingFrame != lastValue)
                {
                    this.velocity += new Vector2(rand.NextSingle() - 0.5f, rand.NextSingle() - 0.5f) * this.walkSpeed * 10;
                }
                if (this.GetPlayerDirection().Length() < this.diveDistance)
                {
                    this.velocity = Vector.SafeNormalize(this.GetPlayerDirection()) * this.walkSpeed * 10;
                }
                else
                {
                    this.velocity = Vector.SafeNormalize(this.GetPlayerDirection()) * this.walkSpeed;
                    this.velocity += new Vector2(rand.NextSingle() - 0.5f, rand.NextSingle() - 0.5f) / 2f;
                }
            }
            else
            {
                float r = 0.1f;
                this.velocity -= this.Node.GetLocalPosition2D() / 10;
            }
        }
        if (this.Node.GetLocalPosition2D().Length() > this.spawner.Radius * 1.5f)
        {
            if (!ping)
            {
                if (rand.NextSingle() > 0.8f)
                {
                    this.velocity -= this.Node.GetLocalPosition2D() / 15;
                    this.ping = true;
                }
            }
        }
        else
        {
            ping = false;
        }

        this.Node.SetLocalPosition(this.Node.GetLocalPosition2D() + this.velocity);
        this.velocity *= 0.7f;
        // PrintLn((this.Node.GetLocalPosition2D() + this.velocity).ToString());
        this.Game.Services.GetService<IPebbleRendererService>().drawDebug(new DebugDrawable(this.Node.GetGlobalPosition2D(), 30, this.isGrounded > 3 ? Color.Orange : Color.Aqua));

        lastValue = this.spawner.PlayerTouchingFrame;
    }
}

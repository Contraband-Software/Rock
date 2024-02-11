namespace GameDemo1.Scripts;

using System.Collections.Generic;
using GREngine.Algorithms;
using GREngine.Core.PebbleRenderer;
using GREngine.Core.Physics2D;
using GREngine.Core.System;
using GREngine.GameBehaviour.Pathfinding;
using Microsoft.Xna.Framework;

public class Enemy : Behaviour
{
    #region SETTINGS
    private float diveDistance = 2;
    private float walkSpeed = 10;
    #endregion

    #region STATE
    public float DestinationDistanceToPlayer => (this.player.Node.GetGlobalPosition2D() - this.tracePath[^1].ToVector2()).Length();

    private PathfindingSearchNetwork network;
    private PlayerController player;
    private CircleCollider collider;

    private List<Point> tracePath = new List<Point>();
    #endregion

    protected override void OnStart()
    {
        ISceneControllerService sc = this.Game.Services.GetService<ISceneControllerService>();
        this.network = sc
            .FindNodeWithTag("GREngine.GameBehaviour.Pathfinding.NodeNetwork")!
            .GetBehaviour<PathfindingSearchNetwork>() as PathfindingSearchNetwork;
        this.player = sc
            .FindNodeWithTag("Player")!
            .GetBehaviour<PlayerController>() as PlayerController;

        collider = this.Game.Services.GetService<ISceneControllerService>().InitBehaviour(this.Node, new CircleCollider(50, true)) as CircleCollider;
        this.collider.SetTrigger(true);
    }

    private void MakePathToPlayer()
    {
        Vector2 pos = this.Node.GetGlobalPosition2D();
        Vector2 playerPos = this.player.Node.GetGlobalPosition2D();
        tracePath = this.network.Navigate(new Point((int)pos.X, (int)pos.Y), new Point((int)playerPos.X, (int)playerPos.Y));
    }

    protected override void OnUpdate(GameTime gameTime)
    {
        if (this.tracePath.Count > 0)
        {
            if (DestinationDistanceToPlayer > this.diveDistance)
            {
                MakePathToPlayer();
            }
            else
            {

            }
        }
        else
        {
            MakePathToPlayer();
        }

        // if (this.tracePath.Count > 0)
        // {
        //     if (this.currentlyGoingTo == null)
        //     {
        //         currentlyGoingTo = this.tracePath[0];
        //         this.tracePath.RemoveAt(0);
        //     }
        //     else
        //     {
        //         if ((currentlyGoingTo.Value.ToVector2() - this.Node.GetGlobalPosition2D()).Length() < 20)
        //         {
        //             currentlyGoingTo = this.tracePath[0];
        //             this.tracePath.RemoveAt(0);
        //         }
        //
        //         Vector2 go = currentlyGoingTo.Value.ToVector2() - this.Node.GetGlobalPosition2D();
        //         // go = Vector.SafeNormalize(go);
        //
        //         this.collider.SetVelocity(go * walkSpeed * 100);
        //     }
        // }
    }

    private Point? currentlyGoingTo = null;
}

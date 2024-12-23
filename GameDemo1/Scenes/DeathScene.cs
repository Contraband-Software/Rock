namespace GameDemo1.Scenes;

using System.Collections.Generic;
using GREngine.Core.PebbleRenderer;
using GREngine.Core.Physics2D;
using GREngine.Core.System;
using GREngine.GameBehaviour.Pathfinding;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Scripts;

public class DeathScene : Scene
{
    public DeathScene() : base("DeathScene")
    {

    }

    protected override void OnLoad(SceneManager sceneManager)
    {
        ISceneManager s = Game.Services.GetService<ISceneManager>();

        NodePointer root = s.AddNodeAtRoot("DeathSceneContainer");

        NodePointer gameOver = s.AddNode(root, "gameOver");
        NodePointer restart = s.AddNode(root, "restart");

        gameOver.SetLocalPosition(550, 300);
        restart.SetLocalPosition(550, 400);

        UIElement gameOverUI = new UIElement(
            "Game Over!", Game.Content.Load<SpriteFont>("Graphics/CRTFont"), Color.Red, 2);
        UIElement restartUI = new UIElement(
            "Press R \n to restart.", Game.Content.Load<SpriteFont>("Graphics/CRTFont"), Color.Red, 2);
        DeathSceneController controller = new DeathSceneController();

        s.AddBehaviour(gameOver, gameOverUI);
        s.AddBehaviour(gameOver, controller);
        s.AddBehaviour(restart, restartUI);
    }
}

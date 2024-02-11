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
        GenericNode root = new GenericNode();
        GenericNode gameOver = new GenericNode("gameOver");
        GenericNode restart = new GenericNode("restart");


        gameOver.SetLocalPosition(550, 300);
        restart.SetLocalPosition(550, 400);

        UIElement gameOverUI = new UIElement("Game Over!", Game.Content.Load<SpriteFont>("Graphics/CRTFont"),Color.Red,2);
        UIElement restartUI = new UIElement("Press R \nto restart.", Game.Content.Load<SpriteFont>("Graphics/CRTFont"), Color.Red, 2);
        DeathSceneController controller = new DeathSceneController();

        Game.Services.GetService<ISceneControllerService>().AddBehaviour(gameOver, gameOverUI);
        Game.Services.GetService<ISceneControllerService>().AddBehaviour(gameOver, controller);
        Game.Services.GetService<ISceneControllerService>().AddBehaviour(restart, restartUI);


        Game.Services.GetService<ISceneControllerService>().AddNodeAtRoot(root);
        Game.Services.GetService<ISceneControllerService>().AddNode(root,gameOver);
        Game.Services.GetService<ISceneControllerService>().AddNode(root, restart);

    }
}

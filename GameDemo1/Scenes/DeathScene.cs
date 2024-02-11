namespace Testing.SystemTesting;

using GREngine.Core.PebbleRenderer;
using GREngine.Core.System;
using GREngine.Debug;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;



public class GraphicsTestScene : Scene
{
    public GraphicsTestScene() : base("GraphicsTestScene")
    {

    }

    protected override void OnLoad(SceneManager sceneManager)
    {
        GenericNode root = new GenericNode();
        GenericNode gameOver = new GenericNode();

        UIElement gameOverUI = new UIElement("Game Over!", Game.Content.Load<SpriteFont>("Graphics/DefaultFont"),Color.Red,2);

        Game.Services.GetService<ISceneControllerService>().AddBehaviour(gameOver, gameOverUI);


        Game.Services.GetService<ISceneControllerService>().AddNodeAtRoot(root);
        Game.Services.GetService<ISceneControllerService>().AddNode(root,gameOver);


    }
}

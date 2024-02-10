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
        GenericNode testObject = new GenericNode();
        GenericNode testObject2 = new GenericNode();
        testObject2.SetLocalPosition(new Vector2(512, 512));
        GenericNode light = new GenericNode();
        GenericNode light2 = new GenericNode();
        light.SetLocalPosition(new Vector2(400, 400));
        light2.SetLocalPosition(new Vector2(900, 600));
        Out.PrintLn("hellow world");


        Sprite spriteRenderer2 = new Sprite(0f, new Vector2(1f),
            Game.Content.Load<Texture2D>("Graphics/Platform1Diffuse"),
            Game.Content.Load<Texture2D>("Graphics/Platform1Normal"),
            Game.Content.Load<Texture2D>("Graphics/Platform1Roughness"), 2, 2, false);

        Sprite spriteRenderer = new Sprite(0f, new Vector2(16f),
            Game.Content.Load<Texture2D>("Graphics/waterColor"),
           Game.Content.Load<Texture2D>("Graphics/waterNormal"),
            null, 1, 1, false);

        Light lightRenderer = new Light(new Vector3(400000, 400000, 360000), true);
        Light lightRenderer2 = new Light(new Vector3(400000, 200000, 200000), true,0,1);

        this.Game.Services.GetService<ISceneControllerService>().AddBehaviour(testObject, spriteRenderer);
        this.Game.Services.GetService<ISceneControllerService>().AddBehaviour(testObject2, spriteRenderer2);

        this.Game.Services.GetService<ISceneControllerService>().AddBehaviour(light, lightRenderer);
        this.Game.Services.GetService<ISceneControllerService>().AddBehaviour(light2, lightRenderer2); // check this expected behaviour???

        this.Game.Services.GetService<ISceneControllerService>().AddNodeAtRoot(root);
        this.Game.Services.GetService<ISceneControllerService>().AddNode(testObject, root);
        this.Game.Services.GetService<ISceneControllerService>().AddNode(testObject2, root);
        this.Game.Services.GetService<ISceneControllerService>().AddNode(light, root);
        this.Game.Services.GetService<ISceneControllerService>().AddNode(light2, root); //bug?

    }
}

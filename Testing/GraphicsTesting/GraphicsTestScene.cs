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
        GenericNode ocean = new GenericNode();
        GenericNode testObject = new GenericNode();
        GenericNode player = new GenericNode();
        player.SetLocalPosition(new Vector2(200));
        testObject.SetLocalPosition(new Vector2(1700, 1000));
        GenericNode testObject2 = new GenericNode();
        testObject2.SetLocalPosition(new Vector2(512, 512));
        GenericNode light = new GenericNode();
        GenericNode light2 = new GenericNode();
        light.SetLocalPosition(new Vector2(512, 512));
        light2.SetLocalPosition(new Vector2(1500, 600));
        //Out.PrintLn("hellow world");


        Sprite platformRenderer1 = new Sprite(new Vector2(0),0f, new Vector2(1f),
            Game.Content.Load<Texture2D>("Graphics/Platform1Diffuse"),
            Game.Content.Load<Texture2D>("Graphics/Platform1Normal"),
            Game.Content.Load<Texture2D>("Graphics/Platform1Roughness"), 2, 2, false);

        Sprite platformRenderer2 = new Sprite(new Vector2(60), 0f, new Vector2(1f),
         Game.Content.Load<Texture2D>("Graphics/Platform1Diffuse"),
         Game.Content.Load<Texture2D>("Graphics/Platform1Normal"),
         Game.Content.Load<Texture2D>("Graphics/Platform1Roughness"), 2, 2, false);


        Sprite oceanRenderer = new Sprite(new Vector2(0), 0f, new Vector2(16f),
            Game.Content.Load<Texture2D>("Graphics/waterColor"),
           Game.Content.Load<Texture2D>("Graphics/waterNormal"),
            Game.Content.Load<Texture2D>("Graphics/Platform1Roughness"), 1, 1, false);

        Sprite playerRenderer = new Sprite(new Vector2(100), 0f, new Vector2(0.4f),
           Game.Content.Load<Texture2D>("Graphics/PlayerDiffuse"),
         null,
           null,7, 3, false);

        Light lightRenderer = new Light(new Vector2(0), new Vector3(300000, 300000, 260000), true);
        Light lightRenderer2 = new Light(new Vector2(0), new Vector3(400000, 200000, 200000), true,0,1);

        this.Game.Services.GetService<ISceneControllerService>().AddBehaviour(ocean, oceanRenderer);
        this.Game.Services.GetService<ISceneControllerService>().AddBehaviour(testObject, platformRenderer1);
        this.Game.Services.GetService<ISceneControllerService>().AddBehaviour(testObject2, platformRenderer2);
        this.Game.Services.GetService<ISceneControllerService>().AddBehaviour(player, playerRenderer);

        this.Game.Services.GetService<ISceneControllerService>().AddBehaviour(light, lightRenderer);
        this.Game.Services.GetService<ISceneControllerService>().AddBehaviour(light2, lightRenderer2); // check this expected behaviour???

        this.Game.Services.GetService<ISceneControllerService>().AddNodeAtRoot(root);
        this.Game.Services.GetService<ISceneControllerService>().AddNode(player, root);
        this.Game.Services.GetService<ISceneControllerService>().AddNode(ocean, root);
        this.Game.Services.GetService<ISceneControllerService>().AddNode(testObject, root);
        this.Game.Services.GetService<ISceneControllerService>().AddNode(testObject2, root);
        this.Game.Services.GetService<ISceneControllerService>().AddNode(light, root);
        this.Game.Services.GetService<ISceneControllerService>().AddNode(light2, root); //bug?

    }
}

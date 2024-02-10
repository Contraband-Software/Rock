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

    protected override void OnLoad(Node rootNode, Node persistantNode)
    {
        GenericNode root = new GenericNode();
        GenericNode testObject = new GenericNode();
        GenericNode light = new GenericNode();
        GenericNode light2 = new GenericNode();
        light.SetLocalPosition(new Vector2(0, 0));
        light2.SetLocalPosition(new Vector2(300, 700));
        Out.PrintLn("hellow world");


        //Sprite spriteRenderer = new Sprite(0f, new Vector2(0.5f),
        //    Game.Content.Load<Texture2D>("Graphics/space-cruiser-panels2_albedo"),
        //    Game.Content.Load<Texture2D>("Graphics/space-cruiser-panels2_normal-ogl"),
        //    Game.Content.Load<Texture2D>("Graphics/space-cruiser-panels2_roughness"),0,1,false);

        Sprite spriteRenderer = new Sprite(0f, new Vector2(1f),
            Game.Content.Load<Texture2D>("Graphics/space-cruiser-panels2_albedo"),
           null,
            null, 0, 1, false);

        Light lightRenderer = new Light(new Vector3(2800000, 2800000, 2000000), true, 1f, 0.9f);
        Light lightRenderer2 = new Light(new Vector3(280000, 10000, 100000), true);

        this.Game.Services.GetService<ISceneControllerService>().AddBehaviour(testObject, spriteRenderer);
        this.Game.Services.GetService<ISceneControllerService>().AddBehaviour(light, lightRenderer);
        //this.Game.Services.GetService<ISceneControllerService>().AddBehaviour(light2, lightRenderer2);

        this.Game.Services.GetService<ISceneControllerService>().AddNodeAtRoot(root);
        this.Game.Services.GetService<ISceneControllerService>().AddNode(testObject, root);
        this.Game.Services.GetService<ISceneControllerService>().AddNode(light, root);
        //this.Game.Services.GetService<ISceneControllerService>().AddNode(light2, root); //bug?

    }
}

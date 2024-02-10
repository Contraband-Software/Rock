namespace Testing.SystemTesting;

using GREngine.Core.PebbleRenderer;
using GREngine.Core.System;
using GREngine.Debug;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

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
        light.SetLocalPosition(new Vector2(400, 400));

        Debug.WriteLine("hellow world");

        Sprite spriteRenderer = new Sprite(0f, new Vector2(1),
            Game.Content.Load<Texture2D>("Graphics/space-cruiser-panels2_albedo"),
            Game.Content.Load<Texture2D>("Graphics/space-cruiser-panels2_normal-ogl"),
            Game.Content.Load<Texture2D>("Graphics/space-cruiser-panels2_roughness"),0,0,false);

        Light lightRenderer = new Light(new Vector3(1000000, 100000, 100000), true, 1f, 0.9f);

        this.Game.Services.GetService<ISceneControllerService>().AddBehaviour(testObject, spriteRenderer);
        this.Game.Services.GetService<ISceneControllerService>().AddBehaviour(light, lightRenderer);

        this.Game.Services.GetService<ISceneControllerService>().AddNodeAtRoot(root);
        this.Game.Services.GetService<ISceneControllerService>().AddNode(testObject, root);
        this.Game.Services.GetService<ISceneControllerService>().AddNode(light, root);
    }

}

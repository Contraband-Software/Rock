

namespace GameDemo1.Scenes;

using System;
using System.Collections.Generic;
using GREngine.Core.PebbleRenderer;
using GREngine.Core.Physics2D;
using GREngine.Core.System;
using GREngine.GameBehaviour.Pathfinding;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Scripts;

public class GameScene : Scene
{
    private ISceneControllerService sceneController;
    private IPebbleRendererService rendererService;

    internal static readonly string mapFloorCollisionLayer = "mapFloor";
    internal static readonly string mapWallCollisionLayer = "mapWall";
    internal static readonly string enemyCollisionLayer = "enemy";
    internal static readonly string playerCollisionLayer = "player";

    #region STATE
    private Texture2D platform1Diffuse;
    private Texture2D Platform1Normal;
    private Texture2D Platform1Roughness;
    #endregion

    public GameScene() : base("GameScene")
    {
    }

    private Node MakeLevelBlock(CircleCollider col, Vector2 position)
    {
        GenericNode mapNode = new GenericNode("MapGround");
        Light light = new Light(new Vector3(200000,220000,280000),true,0,1);

        sceneController.AddBehaviour(mapNode, light); 
        col.SetLayer(mapFloorCollisionLayer);
        sceneController.AddBehaviour(mapNode, col);
        mapNode.SetLocalPosition(position);

        float scale = (2 * col.GetRadius()) / 898;
        Sprite platformRenderer = new Sprite(new Vector2(60, 60) * scale, 0f, new Vector2(scale),
            platform1Diffuse,
            Platform1Normal,
            null, 2, 2, false);
        this.sceneController.AddBehaviour(mapNode, platformRenderer);

        EnemySpawner es = new EnemySpawner(col.GetRadius() + 10, (uint)(col.GetRadius() / 100));
        this.sceneController.AddBehaviour(mapNode, es);

        return mapNode;
    }

    protected override void OnLoad(SceneManager sceneManager)
    {
        sceneController = sceneManager;
        rendererService = this.Game.Services.GetService<IPebbleRendererService>();

        #region CONTENT_LOADING
        Texture2D waterDiffuse = Game.Content.Load<Texture2D>("Graphics/waterColor");
        Texture2D waterNormal = Game.Content.Load<Texture2D>("Graphics/waterNormal");
        Texture2D waterRoughness = Game.Content.Load<Texture2D>("Graphics/Platform1Roughness");

        platform1Diffuse = Game.Content.Load<Texture2D>("Graphics/Platform1Diffuse");
        Platform1Normal = Game.Content.Load<Texture2D>("Graphics/Platform1Normal");
        Platform1Roughness = Game.Content.Load<Texture2D>("Graphics/Platform1Roughness");
        #endregion

        #region MANAGERS
        Player n1 = new Player();
        sceneManager.AddNodeAtRoot(n1);

        CircleCollider cc = sceneManager.InitBehaviour(n1, new CircleCollider(40, true)) as CircleCollider;
        cc.SetLayer(playerCollisionLayer);
        cc.SetTrigger(true);
        cc.SetAllowedCollisionLayers(new List<string>(){GameScene.mapFloorCollisionLayer, GameScene.enemyCollisionLayer, GameScene.mapWallCollisionLayer});
        PlayerController pc = new PlayerController(mapFloorCollisionLayer);
        sceneManager.AddBehaviour(n1, pc);


        GenericNode gameManager = new GenericNode("GameManager");
        GameController gc = new GameController();
        sceneManager.AddBehaviour(gameManager, gc);
        sceneManager.AddNodeAtRoot(gameManager);
        #endregion

        #region MAP
        GenericNode mapRoot = new GenericNode("MapRoot");
        sceneManager.AddNodeAtRoot(mapRoot);
        Sprite oceanRenderer = new Sprite(0f, new Vector2(16f),
            waterDiffuse,
            waterNormal,
            null, 1, 1, false);
        sceneManager.AddBehaviour(mapRoot, oceanRenderer);

        // this.sceneController.AddNode(mapRoot, MakeLevelBlock(new CircleCollider(400), new Vector2(100, 101)));
        // this.sceneController.AddNode(mapRoot, MakeLevelBlock(new CircleCollider(240), new Vector2(-600, -700)));
        // this.sceneController.AddNode(mapRoot, MakeLevelBlock(new CircleCollider(200), new Vector2(80, 700)));
        // this.sceneController.AddNode(mapRoot, MakeLevelBlock(new CircleCollider(200), new Vector2(1000, -170)));
        // this.sceneController.AddNode(mapRoot, MakeLevelBlock(new CircleCollider(150), new Vector2(-900, 570)));

        List<float> radii = new List<float>();
        List<Vector2> poses = new List<Vector2>();

        float biggestPlat = 0;
        Vector2 biggestCentre = Vector2.Zero;

        Random r = new Random();
        for (int i = 0; i < 15; i++)
        {
            Vector2 pos = Vector2.Zero;
            float radius = 0;

            bool goodSpawn = false;
            while (!goodSpawn)
            {
                goodSpawn = true;
                pos = new Vector2(r.NextSingle() * 4000, r.NextSingle() * 4000);
                radius = r.NextSingle() * 350 + 150;
                for (int j = 0; j < radii.Count; j++)
                {
                    if ((poses[j] - pos).Length() < radius + radii[j] + 50)
                    {
                        goodSpawn = false;
                    }
                }
            }
            radii.Add(radius);
            poses.Add(pos);
            this.sceneController.AddNode(mapRoot, MakeLevelBlock(new CircleCollider(radius), pos));

            if (radius > biggestPlat)
            {
                biggestPlat = radius;
                biggestCentre = pos;
            }
        }

        n1.SetLocalPosition(biggestCentre + new Vector2(biggestPlat) * 0.7f);

        #region LIGHTING
        Light lightRenderer = new Light(new Vector2(101, 100), new Vector3(1, 1, 1) * 100000, false);
        sceneManager.AddBehaviour(mapRoot, lightRenderer);
        #endregion
        #endregion

        sceneManager.QueueSceneAction(_ =>
        {
            sceneManager.DebugPrintGraph();
        });
    }
}

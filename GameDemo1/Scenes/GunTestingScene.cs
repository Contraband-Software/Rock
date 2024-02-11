namespace GameDemo1.Scenes
{
    using GameDemo1.Scripts;
    using GREngine.Core.PebbleRenderer;
    using GREngine.Core.Physics2D;
    using GREngine.Core.System;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class GunTestingScene : Scene
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

        public GunTestingScene() : base("GunTestingScene")
        {
        }

        private Node MakeLevelBlock(CircleCollider col, Vector2 position)
        {
            GenericNode mapNode = new GenericNode("MapGround");
            col.SetTrigger(true);
            col.SetLayer(mapFloorCollisionLayer);
            sceneController.AddBehaviour(mapNode, col);
            mapNode.SetLocalPosition(position);

            float scale = (2 * col.GetRadius()) / 898;
            Sprite platformRenderer = new Sprite(new Vector2(60, 60) * scale, 0f, new Vector2(scale),
                platform1Diffuse,
                Platform1Normal,
                null, 2, 2, false);
            this.sceneController.AddBehaviour(mapNode, platformRenderer);

            EnemySpawner es = new EnemySpawner(col.GetRadius() + 10, 4);
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
            n1.SetLocalPosition(-100, 140);

            CircleCollider cc = sceneManager.InitBehaviour(n1, new CircleCollider(40, true)) as CircleCollider;
            cc.SetLayer(playerCollisionLayer);
            cc.SetAllowedCollisionLayers(new List<string>() { GameScene.mapFloorCollisionLayer, GameScene.enemyCollisionLayer, GameScene.mapWallCollisionLayer });

            PlayerController pc = new PlayerController(mapFloorCollisionLayer);
            sceneManager.AddBehaviour(n1, pc);
            #endregion

            #region MAP
            GenericNode mapRoot = new GenericNode("MapRoot");
            sceneManager.AddNodeAtRoot(mapRoot);

            this.sceneController.AddNode(mapRoot, MakeLevelBlock(new CircleCollider(400), new Vector2(100, 101)));
            this.sceneController.AddNode(mapRoot, MakeLevelBlock(new CircleCollider(240), new Vector2(-600, -700)));
            this.sceneController.AddNode(mapRoot, MakeLevelBlock(new CircleCollider(200), new Vector2(80, 700)));
            this.sceneController.AddNode(mapRoot, MakeLevelBlock(new CircleCollider(200), new Vector2(1000, -170)));
            this.sceneController.AddNode(mapRoot, MakeLevelBlock(new CircleCollider(150), new Vector2(-900, 570)));

            #region LIGHTING
            Light lightRenderer = new Light(new Vector2(101, 100), new Vector3(1, 1, 1) * 100000, false);
            sceneManager.AddBehaviour(mapRoot, lightRenderer);
            #endregion
            #endregion

            GenericNode enemy1 = new GenericNode();
            sceneManager.AddNodeAtRoot(enemy1);
            enemy1.SetLocalPosition(100, 300);
            CircleCollider enemCirc1 = sceneManager.InitBehaviour(enemy1, new CircleCollider(40, true)) as CircleCollider;
            enemCirc1.SetLayer(enemyCollisionLayer);

            sceneManager.QueueSceneAction(_ =>
            {
                sceneManager.DebugPrintGraph();
            });
        }
    }
}

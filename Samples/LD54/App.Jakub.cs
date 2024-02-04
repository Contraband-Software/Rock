namespace LD54;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Engine;
using Microsoft.Xna.Framework.Graphics;
using LD54.Engine.Leviathan;
using LD54.Engine.Collision;
using Engine.Components;
using LD54.AsteroidGame.GameObjects;
using System.Collections.Generic;
using System;

class LevelSquare : GameObject
{
    Texture2D texture;
    RigidBodyComponent rb;
    private float scale = 1f;
    public LevelSquare(Texture2D texture, float scale, string name, Game appCtx) : base(name, appCtx)
    {
        this.texture = texture;
        this.scale = scale;
    }

    public override void OnLoad(GameObject? parentObject)
    {
        SpriteRendererComponent src = new SpriteRendererComponent("texture", this.app);
        src.LoadSpriteData(
            this.GetGlobalTransform(),
            new Vector2(
                (int)((this.texture.Width) * scale),
                (int)((this.texture.Height) * scale)),
            this.texture);

        this.AddComponent(src);

        Vector3 colliderDimensions = new Vector3
            ((int)((this.texture.Width) * scale),
            (int)((this.texture.Height) * scale), 0);


        BoxColliderComponent collider = new BoxColliderComponent(colliderDimensions, Vector3.Zero, "squareCollider", this.app);
        this.AddComponent(collider);

        rb = new RigidBodyComponent("rbPlayer", app);
        this.AddComponent(rb);
    }
}


class LevelBlock : GameObject
{
    Texture2D texture;
    RigidBodyComponent rb;
    private float scale = 1f;
    public LevelBlock(Texture2D texture, float scale, string name, Game appCtx) : base(name, appCtx)
    {
        this.texture = texture;
        this.scale = scale;
    }

    public override void OnLoad(GameObject? parentObject)
    {
        SpriteRendererComponent src = new SpriteRendererComponent("texture", this.app);
        src.LoadSpriteData(
            this.GetGlobalTransform(),
            new Vector2(
                ((this.texture.Width) * scale),
                ((this.texture.Height) * scale)),
            this.texture);

        this.AddComponent(src);

        Vector3 colliderDimensions = new Vector3
            ((int)((this.texture.Width) * scale),
            (int)((this.texture.Height) * scale), 0);


        ColliderComponent collider = new CircleColliderComponent(colliderDimensions.X / 2, Vector3.Zero, "blockCollider", this.app);
        this.AddComponent(collider);

        rb = new RigidBodyComponent("rbPlayer", app);
        this.AddComponent(rb);
    }
}


class PlayerBlock : GameObject
{
    Texture2D texture;
    public float Speed = 5f;
    ColliderComponent collider;
    RigidBodyComponent rb;
    public PlayerBlock(Texture2D texture, string name, Game appCtx) : base(name, appCtx)
    {
        this.texture = texture;

        Matrix pos = this.GetLocalTransform();
        pos.Translation = new Vector3(150, 150, 1);

        this.SetLocalTransform(pos);
    }

    public override void OnLoad(GameObject? parentObject)
    {
        SpriteRendererComponent src = new SpriteRendererComponent("Sprite1", this.app);
        src.LoadSpriteData(
            this.GetGlobalTransform(),
            new Vector2(this.texture.Width, this.texture.Height),
            this.texture);

        this.AddComponent(src);

        Vector3 colliderDimensions = new Vector3(this.texture.Width, this.texture.Height, 0);
        Vector3 offset = new Vector3(0, 0, 0);
        collider = new CircleColliderComponent(colliderDimensions.X / 2, offset, "playerCollider", this.app);
        this.AddComponent(collider);

        rb = new RigidBodyComponent("rbPlayer", app);
        this.AddComponent(rb);

        collider.TriggerEvent += OnTriggerEnter;
    }

    private void OnTriggerEnter(ColliderComponent other)
    {
        PrintLn("TRIGGERED ON: " + other.GetName());

        //see if gameobject collided with is of type Asteroid
        //if so, start its death countdown
        if(other.GetGameObject() is Asteroid)
        {
            Asteroid asteroid = (Asteroid)other.GetGameObject();
            if(asteroid != null)
            {
                if(asteroid.state == Asteroid.State.ALIVE)
                {
                    asteroid.StartDeathCountdown();
                    //get velocity of asteroid
                    Vector3 asteroidVel = ((RigidBodyComponent)asteroid.GetComponent<RigidBodyComponent>()).Velocity;

                    //get position delta from asteroid to player
                    Vector3 asteroidPos = asteroid.GetGlobalPosition();
                    Vector3 thisPos = GetGlobalPosition();
                    Vector3 posDelta = new Vector3(
                        thisPos.X - asteroidPos.X,
                        thisPos.Y - asteroidPos.Y,
                        0);

                    float dotProduct = Vector3.Dot(asteroidVel, posDelta);
                    if (dotProduct > 0)
                    {
                        rb.Velocity += asteroidVel;
                    }

                    //otherwise, damp current velocity
                }
            }
        }
    }
    private void OnCollisionEnter(ColliderComponent other)
    {
        PrintLn("COLLIDED WITH: " + other.GetName());
    }

    public override void Update(GameTime gameTime)
    {
        //rb.Velocity = Vector3.Zero;
        Move();

        base.Update(gameTime);
    }


    private void Move()
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Left))
        {
            rb.Velocity.X -= Speed;
        }
        if (Keyboard.GetState().IsKeyDown(Keys.Right))
        {
            rb.Velocity.X += Speed;
        }

        if (Keyboard.GetState().IsKeyDown(Keys.Up))
        {
            rb.Velocity.Y -= Speed;
        }
        if (Keyboard.GetState().IsKeyDown(Keys.Down))
        {
            rb.Velocity.Y += Speed;
        }
    }
}

class JakubScene : Scene
{
    public JakubScene(Game appCtx) : base("JakubScene", appCtx)
    {
    }

    public override void OnLoad(GameObject? parentObject)
    {
        Texture2D blankTexure = this.contentManager.Load<Texture2D>("Sprites/circle");
        Texture2D squong = this.contentManager.Load<Texture2D>("Sprites/block");
        Texture2D arrow = this.contentManager.Load<Texture2D>("Sprites/arrow");

        Texture2D asteroidTex1 = this.contentManager.Load<Texture2D>("Sprites/asteroid_1");
        Texture2D asteroidTex2 = this.contentManager.Load<Texture2D>("Sprites/asteroid_2");
        Texture2D asteroidTex3 = this.contentManager.Load<Texture2D>("Sprites/asteroid_3");
        Texture2D asteroid_broken = this.contentManager.Load<Texture2D>("Sprites/asteroid_broken");
        List<Texture2D> asteroids = new List<Texture2D>() { asteroidTex1, asteroidTex2, asteroidTex3};

        //PlayerBlock playerBlock = new PlayerBlock(blankTexure, "spovus", app);
        //parentObject.AddChild(playerBlock);

        /*        Spaceship player = new Spaceship(null, arrow, "spovus", app);
                parentObject.AddChild(player);*/


        LevelBlock levelBlock = new LevelBlock(blankTexure,1f, "spovus", app);
        levelBlock.SetLocalPosition(new Vector3(600, 300, 1));
        parentObject.AddChild(levelBlock);

        LevelBlock levelBlock2 = new LevelBlock(blankTexure, 2, "spovus", app);
        levelBlock2.SetLocalPosition(new Vector3(440, 150, 1));
        parentObject.AddChild(levelBlock2);

        LevelSquare levelSquare = new LevelSquare(squong, 1, "spovus", app);
        levelSquare.SetLocalPosition(new Vector3(540, 50, 1));
        parentObject.AddChild(levelSquare);

        PlayerBlock playerBlock = new PlayerBlock(blankTexure, "player", app);
        playerBlock.SetLocalPosition(new Vector3(300, 300, 1));
        parentObject.AddChild(playerBlock);


        Random rnd = new Random();
        Vector3 vel = new Vector3(1, 1, 0);
        Asteroid asteroid1 = new Asteroid(0, vel, asteroids[rnd.Next(0, 3)], asteroid_broken, "asteroid", app);
        asteroid1.SetLocalPosition(new Vector3(0, 0, 1));
        parentObject.AddChild(asteroid1);

        Asteroid asteroid2 = new Asteroid(0, vel, asteroids[rnd.Next(0, 3)], asteroid_broken, "asteroid", app);
        asteroid2.SetLocalPosition(new Vector3(80, 0, 1));
        parentObject.AddChild(asteroid2);

        Asteroid asteroid3 = new Asteroid(0, vel, asteroids[rnd.Next(0, 3)], asteroid_broken, "asteroid", app);
        asteroid3.SetLocalPosition(new Vector3(0, 100, 1));
        parentObject.AddChild(asteroid3);

        this.app.Services.GetService<ILeviathanEngineService>().AddLight(new Vector2(200, 200), new Vector3(10000000, 10000000, 10000000));
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }

    public override void OnUnload() => throw new System.NotImplementedException();
}


public class App_Jakub : Game
{
    private GraphicsDeviceManager graphics;
    private SceneController sc;
    private LeviathanEngine le;
    private CollisionSystem cs;

    public App_Jakub()
    {
        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

    }

    protected override void Initialize()
    {
        le = new LeviathanEngine(this);
        this.Components.Add(le);
        this.Services.AddService(typeof(ILeviathanEngineService), le);

        sc = new SceneController(this);
        this.Components.Add(sc);
        this.Services.AddService(typeof(ISceneControllerService), sc);

        cs = new CollisionSystem(this);
        this.Components.Add(cs);
        this.Services.AddService(typeof(ICollisionSystemService), cs);


        sc.AddScene(new JakubScene(this));
        sc.ChangeScene("JakubScene");

        base.Initialize();
    }

    protected override void LoadContent()
    {
    }

    protected override void Update(GameTime gameTime)
    {

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
    }
}


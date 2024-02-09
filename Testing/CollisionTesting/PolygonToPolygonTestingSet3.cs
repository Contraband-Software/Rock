using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Drawing;
using Color = Microsoft.Xna.Framework.Color;

using GREngine.Core.Physics2D;

public class PolygonToPolygonTestingSet3 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Texture2D _texture;
    private Texture2D pixelTexture;
    private CollisionSystem collisionSystem;

    #region A_TESTING_VARS
    VerletObject player;
    VerletObject cola1;
    VerletObject cola2;
    VerletObject cola3;
    #endregion


    //mouse
    MouseState lastMouseState;
    MouseState currentMouseState;

    public PolygonToPolygonTestingSet3()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _graphics.PreferredBackBufferWidth = GraphicsDevice.Adapter.CurrentDisplayMode.Width;
        _graphics.PreferredBackBufferHeight = GraphicsDevice.Adapter.CurrentDisplayMode.Height;
        _graphics.ApplyChanges();

        // TODO: Add your initialization logic here
        collisionSystem = CollisionSystem.Instance;
        Services.AddService<ICollisionSystem>(collisionSystem);
        collisionSystem.SetPosition(new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2));

        List<PointF> squarePointFList = new List<PointF>
        {
            new PointF(-20, -20),
            new PointF(20, -20),
            new PointF(20, 20),
            new PointF(-20, 20)
        };
        List<PointF> rectanglePointFList = new List<PointF>{
            new PointF(-40, -20),
            new PointF(40, -20),
            new PointF(40, 20),
            new PointF(-40, 20)
        };
        List<PointF> diamondPointFList = new List<PointF>
        {
            new PointF(0, -20),
            new PointF(20, 0),
            new PointF(0, 20),
            new PointF(-20, 0),
        };

        #region LineSegmentOverlapTesting
        {
            PointF a1 = new PointF(4, 3);
            PointF a2 = new PointF(2, -1);
            PointF b1 = new PointF(1, 1);
            PointF b2 = new PointF(5, 1);
            Debug.WriteLine(CollisionSystem.LinesSegmentsOverlap(a1, a2, b1, b2) == true);
        }
        {
            PointF a1 = new PointF(4, 3);
            PointF a2 = new PointF(2, -1);
            PointF b1 = new PointF(1, 1);
            PointF b2 = new PointF(5, 0);
            //Debug.WriteLine(CollisionSystem.LinesSegmentsOverlap(a1, a2, b1, b2) == true);
        }
        {
            PointF a1 = new PointF(4, 6);
            PointF a2 = new PointF(2, 2);
            PointF b1 = new PointF(1, 1);
            PointF b2 = new PointF(5, 1);
            //Debug.WriteLine(CollisionSystem.LinesSegmentsOverlap(a1, a2, b1, b2) == false);
        }
        {
            PointF a1 = new PointF(4, 3);
            PointF a2 = new PointF(2, -1);
            PointF b1 = new PointF(3, 6);
            PointF b2 = new PointF(3, -3);
            //Debug.WriteLine(CollisionSystem.LinesSegmentsOverlap(a1, a2, b1, b2) == true);
        }
        #endregion
        #region LineSegmentParallelTesting
        {
            Line l1 = new Line(new PointF(3, 6), new PointF(3, -3));
            Line l2 = new Line(new PointF(4, 6), new PointF(4, -3));
            //Debug.WriteLine(CollisionSystem.LinesCanIntersect(l1,l2) == false);
        }
        {
            Line l1 = new Line(new PointF(4, 3), new PointF(2, -1));
            Line l2 = new Line(new PointF(6, 3), new PointF(4, -1));
            //Debug.WriteLine(CollisionSystem.LinesCanIntersect(l1, l2) == false);
        }
        {
            Line l1 = new Line(new PointF(4, 3), new PointF(2, -1));
            Line l2 = new Line(new PointF(3, 6), new PointF(3, -3));
            //Debug.WriteLine(CollisionSystem.LinesCanIntersect(l1, l2) == true);
        }
        #endregion
        #region LineIntersectionTesting
        {
            Line l1 = new Line(new PointF(4, 3), new PointF(2, -1));
            Line l2 = new Line(new PointF(1, 1), new PointF(5, 1));
            //Debug.WriteLine(CollisionSystem.LineIntersectionPointF(l1, l2));
        }
        {
            Line l1 = new Line(new PointF(4, 3), new PointF(2, -1));
            Line l2 = new Line(new PointF(3, 6), new PointF(3, -3));
            //Debug.WriteLine(CollisionSystem.LineIntersectionPointF(l1, l2));
        }
        {
            Line l1 = new Line(new PointF(1, 1), new PointF(5, 1));
            Line l2 = new Line(new PointF(3, 6), new PointF(3, -3));
            //Debug.WriteLine(CollisionSystem.LineIntersectionPointF(l1, l2));
        }
        #endregion
        #region IntersectionInLineSegementsTesting
        {
            PointF a1 = new PointF(4, 3);
            PointF a2 = new PointF(2, -1);
            PointF b1 = new PointF(1, 1);
            PointF b2 = new PointF(5, 1);
            //Debug.WriteLine(CollisionSystem.IntersectionIsWithinLineSegments(new PointF(3,1), a1, a2, b1, b2) == true);
        }
        {
            PointF a1 = new PointF(4, 3);
            PointF a2 = new PointF(2, -1);
            PointF b1 = new PointF(4, 1);
            PointF b2 = new PointF(8, 1);
            //Debug.WriteLine(CollisionSystem.IntersectionIsWithinLineSegments(new PointF(3, 1), a1, a2, b1, b2) == false);
        }
        #endregion
        #region AABBOverlapTesting
        {
            PointF minA = new PointF(4, 2);
            PointF maxA = new PointF(6, 6);
            PointF minB = new PointF(3, 0);
            PointF maxB = new PointF(7, 3);
            AABB a = new AABB(minA, maxA);
            AABB b = new AABB(minB, maxB);
            AABB c = CollisionSystem.GetAABBOverlapRegion(a, b);
            //Debug.WriteLine(c.min == new PointF(4,2));
            //Debug.WriteLine(c.max == new PointF(6,3));

            PointF p = new PointF(5, 2);
            //Debug.WriteLine(CollisionSystem.PointFIsInAABB(p, c));
        }
        {
            PointF minA = new PointF(2, 0);
            PointF maxA = new PointF(4, 2);
            PointF minB = new PointF(3, 1);
            PointF maxB = new PointF(5, 3);
            AABB a = new AABB(minA, maxA);
            AABB b = new AABB(minB, maxB);
            AABB c = CollisionSystem.GetAABBOverlapRegion(a, b);
            //Debug.WriteLine(c.min == new PointF(3,1));
            //Debug.WriteLine(c.max == new PointF(4,2));
        }
        #endregion

        cola1 = new PolygonCollider(new Vector2(600, 600), rectanglePointFList);
        collisionSystem.AddCollisionObject(cola1);
        cola1.SetStatic(true);

        cola2 = new PolygonCollider(new Vector2(800, 800), diamondPointFList);
        collisionSystem.AddCollisionObject(cola2);
        cola2.SetStatic(true);

        cola3 = new PolygonCollider(new Vector2(840, 800), diamondPointFList);
        collisionSystem.AddCollisionObject(cola3);
        cola3.SetStatic(true);
        cola3.SetVelocity(new Vector2(-0.2f, -0.2f));

        player = new PolygonCollider(new Vector2(600, 560), squarePointFList);
        collisionSystem.AddCollisionObject(player);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _texture = this.Content.Load<Texture2D>("collision/circle");
        pixelTexture = Content.Load<Texture2D>("collision/pixel");

    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        if (Keyboard.GetState().IsKeyDown(Keys.A))
        {
            player.SetVelocity(new Vector2(-0.5f, 0f));
        }
        if (Keyboard.GetState().IsKeyDown(Keys.D))
        {
            player.SetVelocity(new Vector2(0.5f, 0f));
            //player.accelerate(new Vector2(1000f, -10000000f));
        }

        // TODO: Add your update logic here
        collisionSystem.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

        // The active state from the last frame is now old
        lastMouseState = currentMouseState;

        // Get the mouse state relevant for this frame
        currentMouseState = Mouse.GetState();

        // Recognize a single click of the left mouse button
        if (lastMouseState.LeftButton == ButtonState.Released && currentMouseState.LeftButton == ButtonState.Pressed)
        {
            // React to click
            Random random = new Random();
            int randomNumber = random.Next(10, 37);
            //VerletObject obj1 = new CircleCollider(new Vector2(currentMouseState.X, currentMouseState.Y), randomNumber);
            //collisionSystem.AddCollisionObject(obj1);
        }


        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(new Color(31, 31, 31));

        // TODO: Add your drawing code here
        _spriteBatch.Begin();

        //drawing polygons
        foreach (PolygonCollider obj in collisionSystem.GetVerletObjects().OfType<PolygonCollider>())
        {
            obj.DrawDebug(_spriteBatch, pixelTexture);
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}



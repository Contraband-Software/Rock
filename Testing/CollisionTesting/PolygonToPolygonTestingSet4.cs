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

public class PolygonToPolygonTestingSet4 : Game
{
    private GraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch;

    private Texture2D texture;
    private Texture2D pixelTexture;
    private CollisionSystem collisionSystem;

    #region A_TESTING_VARS
    VerletObject cola1_1;
    VerletObject cola1_2;
    VerletObject cola2_1;
    VerletObject cola2_2;
    VerletObject cola3_1;
    VerletObject cola3_2;
    VerletObject cola4_1;
    VerletObject cola4_2;
    VerletObject cola4_3;
    VerletObject cola5_1;
    VerletObject cola5_2;
    VerletObject cola5_3;
    VerletObject cola6_1;
    VerletObject cola6_2;
    VerletObject cola6_3;
    VerletObject cola7_1;
    VerletObject cola7_2;
    VerletObject cola7_3;
    VerletObject cola8_1;
    VerletObject cola8_2;
    VerletObject cola9_1;
    VerletObject cola9_2;
    #endregion
    #region B_TESTING_VARS
    VerletObject colb1_1;
    VerletObject colb1_2;
    VerletObject colb2_1;
    VerletObject colb2_2;
    VerletObject colb3_1;
    VerletObject colb3_2;
    VerletObject colb4_1;
    VerletObject colb4_2;
    VerletObject colb5_1;
    VerletObject colb5_2;
    VerletObject colb6_1;
    VerletObject colb6_2;
    VerletObject colb7_1;
    VerletObject colb7_2;
    VerletObject colb8_1;
    VerletObject colb8_2;
    VerletObject colb9_1;
    VerletObject colb9_2;
    #endregion
    #region C_TESTING_VARS
    VerletObject colc1_1;
    VerletObject colc1_2;
    VerletObject colc2_1;
    VerletObject colc2_2;
    VerletObject colc3_1;
    VerletObject colc3_2;
    VerletObject colc4_1;
    VerletObject colc4_2;
    VerletObject colc5_1;
    VerletObject colc5_2;
    VerletObject colc6_1;
    VerletObject colc6_2;
    VerletObject colc7_1;
    VerletObject colc7_2;
    VerletObject colc8_1;
    VerletObject colc8_2;
    VerletObject colc9_1;
    VerletObject colc9_2;
    #endregion
    #region D_TESTING_VARS
    VerletObject cold1_1;
    VerletObject cold1_2;
    VerletObject cold2_1;
    VerletObject cold2_2;
    VerletObject cold3_1;
    VerletObject cold3_2;
    VerletObject cold4_1;
    VerletObject cold4_2;
    VerletObject cold5_1;
    VerletObject cold5_2;
    VerletObject cold6_1;
    VerletObject cold6_2;
    VerletObject cold7_1;
    VerletObject cold7_2;
    VerletObject cold8_1;
    VerletObject cold8_2;
    VerletObject cold9_1;
    VerletObject cold9_2;
    #endregion

    //mouse
    MouseState lastMouseState;
    MouseState currentMouseState;

    public PolygonToPolygonTestingSet4()
    {
        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        collisionSystem = new(this);
    }

    protected override void Initialize()
    {
        graphics.PreferredBackBufferWidth = GraphicsDevice.Adapter.CurrentDisplayMode.Width;
        graphics.PreferredBackBufferHeight = GraphicsDevice.Adapter.CurrentDisplayMode.Height;
        graphics.ApplyChanges();

        // TODO: Add your initialization logic here
        Components.Add(collisionSystem);
        Services.AddService<ICollisionSystem>(collisionSystem);
        collisionSystem.SetPosition(new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2));

        List<PointF> squarePointFList = new List<PointF>
        {
            new PointF(-20, -20),
            new PointF(20, -20),
            new PointF(20, 20),
            new PointF(-20, 20)
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

        //A Testing
        #region A_TESTING_INIT
        cola1_1 = new PolygonCollider(new Vector2(100, 100), squarePointFList);
        collisionSystem.AddCollisionObject(cola1_1);
        cola1_2 = new PolygonCollider(new Vector2(100, 160), squarePointFList);
        collisionSystem.AddCollisionObject(cola1_2);

        cola2_1 = new PolygonCollider(new Vector2(300, 100), squarePointFList);
        collisionSystem.AddCollisionObject(cola2_1);
        cola2_2 = new PolygonCollider(new Vector2(240, 160), squarePointFList);
        collisionSystem.AddCollisionObject(cola2_2);

        cola3_1 = new PolygonCollider(new Vector2(500, 100), squarePointFList);
        collisionSystem.AddCollisionObject(cola3_1);
        cola3_2 = new PolygonCollider(new Vector2(440, 100), squarePointFList);
        collisionSystem.AddCollisionObject(cola3_2);

        cola4_1 = new PolygonCollider(new Vector2(700, 60), squarePointFList);
        collisionSystem.AddCollisionObject(cola4_1);
        cola4_2 = new PolygonCollider(new Vector2(640, 40), squarePointFList);
        collisionSystem.AddCollisionObject(cola4_2);
        cola4_3 = new PolygonCollider(new Vector2(640, 80), squarePointFList);
        collisionSystem.AddCollisionObject(cola4_3);
        cola4_2.SetStatic(true);
        cola4_3.SetStatic(true);

        cola5_1 = new PolygonCollider(new Vector2(900, 50), squarePointFList);
        collisionSystem.AddCollisionObject(cola5_1);
        cola5_2 = new PolygonCollider(new Vector2(840, 40), squarePointFList);
        collisionSystem.AddCollisionObject(cola5_2);
        cola5_3 = new PolygonCollider(new Vector2(840, 80), squarePointFList);
        collisionSystem.AddCollisionObject(cola5_3);
        cola5_2.SetStatic(true);
        cola5_3.SetStatic(true);

        cola6_1 = new PolygonCollider(new Vector2(1100, 60), squarePointFList);
        collisionSystem.AddCollisionObject(cola6_1);
        cola6_2 = new PolygonCollider(new Vector2(1040, 40), squarePointFList);
        collisionSystem.AddCollisionObject(cola6_2);
        cola6_3 = new PolygonCollider(new Vector2(1040, 80), squarePointFList);
        collisionSystem.AddCollisionObject(cola6_3);

        cola7_1 = new PolygonCollider(new Vector2(1300, 70), squarePointFList);
        collisionSystem.AddCollisionObject(cola7_1);
        cola7_2 = new PolygonCollider(new Vector2(1240, 40), squarePointFList);
        collisionSystem.AddCollisionObject(cola7_2);
        cola7_3 = new PolygonCollider(new Vector2(1240, 80), squarePointFList);
        collisionSystem.AddCollisionObject(cola7_3);

        cola8_1 = new PolygonCollider(new Vector2(1500, 100), squarePointFList);
        collisionSystem.AddCollisionObject(cola8_1);
        cola8_2 = new PolygonCollider(new Vector2(1560, 160), squarePointFList);
        collisionSystem.AddCollisionObject(cola8_2);
        cola8_1.SetStatic(true);
        //
        cola9_1 = new PolygonCollider(new Vector2(1700, 100), squarePointFList);
        collisionSystem.AddCollisionObject(cola9_1);
        cola9_2 = new PolygonCollider(new Vector2(1760, 60), squarePointFList);
        collisionSystem.AddCollisionObject(cola9_2);
        cola9_1.SetStatic(true);
        //
        #endregion
        #region B_TESTING_INIT
        colb1_1 = new PolygonCollider(new Vector2(100, 400), squarePointFList);
        colb1_1.SetRotation(45f);
        collisionSystem.AddCollisionObject(colb1_1);
        colb1_2 = new PolygonCollider(new Vector2(100, 460), squarePointFList);
        collisionSystem.AddCollisionObject(colb1_2);
        colb1_1.SetStatic(true);

        colb2_1 = new PolygonCollider(new Vector2(300, 400), squarePointFList);
        colb2_1.SetRotation(45f);
        collisionSystem.AddCollisionObject(colb2_1);
        colb2_2 = new PolygonCollider(new Vector2(240, 460), squarePointFList);
        collisionSystem.AddCollisionObject(colb2_2);
        colb2_1.SetStatic(true);

        colb3_1 = new PolygonCollider(new Vector2(500, 400), squarePointFList);
        colb3_1.SetRotation(45f);
        collisionSystem.AddCollisionObject(colb3_1);
        colb3_2 = new PolygonCollider(new Vector2(440, 400), squarePointFList);
        collisionSystem.AddCollisionObject(colb3_2);
        colb3_1.SetStatic(true);

        colb4_1 = new PolygonCollider(new Vector2(700, 400), squarePointFList);
        colb4_1.SetRotation(45f);
        collisionSystem.AddCollisionObject(colb4_1);
        colb4_2 = new PolygonCollider(new Vector2(640, 340), squarePointFList);
        collisionSystem.AddCollisionObject(colb4_2);
        colb4_1.SetStatic(true);

        colb5_1 = new PolygonCollider(new Vector2(900, 400), squarePointFList);
        colb5_1.SetRotation(45f);
        collisionSystem.AddCollisionObject(colb5_1);
        colb5_2 = new PolygonCollider(new Vector2(900, 340), squarePointFList);
        collisionSystem.AddCollisionObject(colb5_2);
        colb5_1.SetStatic(true);

        colb6_1 = new PolygonCollider(new Vector2(1100, 400), squarePointFList);
        colb6_1.SetRotation(45f);
        collisionSystem.AddCollisionObject(colb6_1);
        colb6_2 = new PolygonCollider(new Vector2(1160, 340), squarePointFList);
        collisionSystem.AddCollisionObject(colb6_2);
        colb6_1.SetStatic(true);

        colb7_1 = new PolygonCollider(new Vector2(1300, 400), squarePointFList);
        colb7_1.SetRotation(45f);
        collisionSystem.AddCollisionObject(colb7_1);
        colb7_2 = new PolygonCollider(new Vector2(1360, 400), squarePointFList);
        collisionSystem.AddCollisionObject(colb7_2);
        colb7_1.SetStatic(true);

        colb8_1 = new PolygonCollider(new Vector2(1500, 400), squarePointFList);
        colb8_1.SetRotation(45f);
        collisionSystem.AddCollisionObject(colb8_1);
        colb8_2 = new PolygonCollider(new Vector2(1560, 360), squarePointFList);
        collisionSystem.AddCollisionObject(colb8_2);
        colb8_1.SetStatic(true);

        colb9_1 = new PolygonCollider(new Vector2(1700, 400), squarePointFList);
        colb9_1.SetRotation(45f);
        collisionSystem.AddCollisionObject(colb9_1);
        colb9_2 = new PolygonCollider(new Vector2(1760, 360), squarePointFList);
        collisionSystem.AddCollisionObject(colb9_2);
        colb9_1.SetStatic(true);
        #endregion
        #region C_TESTING_INIT
        colc1_1 = new PolygonCollider(new Vector2(100, 700), squarePointFList);
        collisionSystem.AddCollisionObject(colc1_1);
        colc1_2 = new PolygonCollider(new Vector2(100, 760), squarePointFList);
        colc1_2.SetRotation(45f);
        collisionSystem.AddCollisionObject(colc1_2);
        colc1_1.SetStatic(true);

        colc2_1 = new PolygonCollider(new Vector2(300, 700), squarePointFList);
        collisionSystem.AddCollisionObject(colc2_1);
        colc2_2 = new PolygonCollider(new Vector2(240, 760), squarePointFList);
        colc2_2.SetRotation(45f);
        collisionSystem.AddCollisionObject(colc2_2);
        colc2_1.SetStatic(true);

        colc3_1 = new PolygonCollider(new Vector2(500, 700), squarePointFList);
        collisionSystem.AddCollisionObject(colc3_1);
        colc3_2 = new PolygonCollider(new Vector2(440, 700), squarePointFList);
        colc3_2.SetRotation(45f);
        collisionSystem.AddCollisionObject(colc3_2);
        colc3_1.SetStatic(true);

        colc4_1 = new PolygonCollider(new Vector2(700, 700), squarePointFList);
        collisionSystem.AddCollisionObject(colc4_1);
        colc4_2 = new PolygonCollider(new Vector2(640, 640), squarePointFList);
        colc4_2.SetRotation(45f);
        collisionSystem.AddCollisionObject(colc4_2);
        colc4_1.SetStatic(true);

        colc5_1 = new PolygonCollider(new Vector2(900, 700), squarePointFList);
        collisionSystem.AddCollisionObject(colc5_1);
        colc5_2 = new PolygonCollider(new Vector2(900, 640), squarePointFList);
        colc5_2.SetRotation(45f);
        collisionSystem.AddCollisionObject(colc5_2);
        colc5_1.SetStatic(true);

        colc6_1 = new PolygonCollider(new Vector2(1100, 700), squarePointFList);
        collisionSystem.AddCollisionObject(colc6_1);
        colc6_2 = new PolygonCollider(new Vector2(1160, 640), squarePointFList);
        colc6_2.SetRotation(45f);
        collisionSystem.AddCollisionObject(colc6_2);
        colc6_1.SetStatic(true);

        colc7_1 = new PolygonCollider(new Vector2(1300, 700), squarePointFList);
        collisionSystem.AddCollisionObject(colc7_1);
        colc7_2 = new PolygonCollider(new Vector2(1360, 700), squarePointFList);
        colc7_2.SetRotation(45f);
        collisionSystem.AddCollisionObject(colc7_2);
        colc7_1.SetStatic(true);

        colc8_1 = new PolygonCollider(new Vector2(1500, 700), squarePointFList);
        collisionSystem.AddCollisionObject(colc8_1);
        colc8_2 = new PolygonCollider(new Vector2(1560, 660), squarePointFList);
        colc8_2.SetRotation(45f);
        collisionSystem.AddCollisionObject(colc8_2);
        colc8_1.SetStatic(true);

        colc9_1 = new PolygonCollider(new Vector2(1700, 700), squarePointFList);
        collisionSystem.AddCollisionObject(colc9_1);
        colc9_2 = new PolygonCollider(new Vector2(1760, 660), squarePointFList);
        colc9_2.SetRotation(45f);
        collisionSystem.AddCollisionObject(colc9_2);
        colc9_1.SetStatic(true);
        #endregion
        #region D_TESTING_INIT
        cold1_1 = new PolygonCollider(new Vector2(100, 900), squarePointFList);
        collisionSystem.AddCollisionObject(cold1_1);
        cold1_2 = new PolygonCollider(new Vector2(50, 940), squarePointFList);
        cold1_2.SetRotation(45f);
        collisionSystem.AddCollisionObject(cold1_2);
        cold1_1.SetStatic(true);

        cold2_1 = new PolygonCollider(new Vector2(300, 900), squarePointFList);
        collisionSystem.AddCollisionObject(cold2_1);
        cold2_2 = new PolygonCollider(new Vector2(350, 940), squarePointFList);
        cold2_2.SetRotation(45f);
        collisionSystem.AddCollisionObject(cold2_2);
        cold2_1.SetStatic(true);

        cold3_1 = new PolygonCollider(new Vector2(500, 900), squarePointFList);
        collisionSystem.AddCollisionObject(cold3_1);
        cold3_2 = new PolygonCollider(new Vector2(450, 940), squarePointFList);
        cold3_1.SetRotation(45f);
        collisionSystem.AddCollisionObject(cold3_2);
        cold3_1.SetStatic(true);

        cold4_1 = new PolygonCollider(new Vector2(700, 900), squarePointFList);
        collisionSystem.AddCollisionObject(cold4_1);
        cold4_2 = new PolygonCollider(new Vector2(740, 950), squarePointFList);
        cold4_1.SetRotation(45f);
        collisionSystem.AddCollisionObject(cold4_2);
        cold4_1.SetStatic(true);

        cold6_1 = new PolygonCollider(new Vector2(900, 900), squarePointFList);
        collisionSystem.AddCollisionObject(cold6_1);
        cold6_2 = new PolygonCollider(new Vector2(840, 920), squarePointFList);
        collisionSystem.AddCollisionObject(cold6_2);
        cold6_1.SetStatic(true);

        cold7_1 = new PolygonCollider(new Vector2(1100, 900), squarePointFList);
        collisionSystem.AddCollisionObject(cold7_1);
        cold7_2 = new PolygonCollider(new Vector2(1070, 840), squarePointFList);
        collisionSystem.AddCollisionObject(cold7_2);
        cold7_1.SetStatic(true);

        List<PointF> wackyPointList1 = new List<PointF>
        {
            new PointF(0, -10f),
            new PointF(40, -30f),
            new PointF(40, 10f),
            new PointF(0, 40f),
            new PointF(-40, 10f),
            new PointF(-40, -30f)

        };
        cold8_1 = new PolygonCollider(new Vector2(1500, 900), wackyPointList1);
        collisionSystem.AddCollisionObject(cold8_1);
        cold8_2 = new PolygonCollider(new Vector2(1490, 840), squarePointFList);
        collisionSystem.AddCollisionObject(cold8_2);
        cold8_1.SetStatic(true);

        #endregion

        base.Initialize();
    }

    protected override void LoadContent()
    {
        spriteBatch = new SpriteBatch(GraphicsDevice);
        texture = this.Content.Load<Texture2D>("collision/circle");
        pixelTexture = Content.Load<Texture2D>("collision/pixel");

    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

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

        #region A_TESTING
        cola1_1.SetVelocity(new Vector2(0, 0.2f));
        cola1_2.SetVelocity(new Vector2(0, -0.2f));

        cola2_2.SetVelocity(new Vector2(0.2f, -0.2f));
        cola2_1.SetVelocity(new Vector2(-0.2f, 0.2f));

        cola3_2.SetVelocity(new Vector2(0.2f, 0));
        cola3_1.SetVelocity(new Vector2(-0.4f, 0));

        cola4_1.SetVelocity(new Vector2(-0.2f, 0));

        cola5_1.SetVelocity(new Vector2(-0.2f, 0f));

        cola6_1.SetVelocity(new Vector2(-0.2f, 0f));
        cola7_1.SetVelocity(new Vector2(-0.2f, -0.1f));

        cola8_2.SetVelocity(new Vector2(-0.2f, -0.2f));
        cola9_2.SetVelocity(new Vector2(-0.2f, 0.2f)); //
        #endregion
        #region B_TESTING
        colb1_2.SetVelocity(new Vector2(0, -0.2f));
        colb2_2.SetVelocity(new Vector2(0.2f, -0.2f));
        colb3_2.SetVelocity(new Vector2(0.2f, 0));
        colb4_2.SetVelocity(new Vector2(0.2f, 0.2f));
        colb5_2.SetVelocity(new Vector2(0, 0.2f));
        colb6_2.SetVelocity(new Vector2(-0.2f, 0.2f));
        colb7_2.SetVelocity(new Vector2(-0.2f, 0));
        colb8_2.SetVelocity(new Vector2(-0.2f, 0.2f));
        colb9_2.SetVelocity(new Vector2(-0.2f, 0.2f));
        #endregion
        #region C_TESTING
        colc1_2.SetVelocity(new Vector2(0, -0.2f));
        colc2_2.SetVelocity(new Vector2(0.2f, -0.2f));
        colc3_2.SetVelocity(new Vector2(0.2f, 0));
        colc4_2.SetVelocity(new Vector2(0.2f, 0.2f));
        colc5_2.SetVelocity(new Vector2(0, 0.2f));
        colc6_2.SetVelocity(new Vector2(-0.2f, 0.2f));
        colc7_2.SetVelocity(new Vector2(-0.2f, 0));
        colc8_2.SetVelocity(new Vector2(-0.2f, 0.133f));
        colc9_2.SetVelocity(new Vector2(-0.2f, 0.133f));
        #endregion
        #region D_TESTING
        cold1_2.SetVelocity(new Vector2(0.166f, -0.133f));
        cold2_2.SetVelocity(new Vector2(-0.166f, -0.133f));
        cold3_2.SetVelocity(new Vector2(0.166f, -0.133f));
        cold4_2.SetVelocity(new Vector2(-0.133f, -0.166f));
        cold6_2.SetVelocity(new Vector2(0.2f, -0.066f));
        cold7_2.SetVelocity(new Vector2(0.1f, 0.2f));
        cold8_2.SetVelocity(new Vector2(0f, 0.33f));
        #endregion

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(new Color(31, 31, 31));

        // TODO: Add your drawing code here
        spriteBatch.Begin();

        //drawing polygons
        foreach (PolygonCollider obj in collisionSystem.GetVerletObjects().OfType<PolygonCollider>())
        {
            obj.DrawDebug(spriteBatch, pixelTexture);
        }

        spriteBatch.End();

        base.Draw(gameTime);
    }
}



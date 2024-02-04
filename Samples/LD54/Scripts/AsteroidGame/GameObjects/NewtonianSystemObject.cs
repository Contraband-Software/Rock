namespace LD54.AsteroidGame.GameObjects;

using System.Collections.Generic;
using Engine.Components;
using LD54.Engine.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class NewtonianSystemObject : GameObject
{
    public NewtonianGravity2DControllerComponent Simulation { get; private set; }
    public float GravitationalConstant { get; private set; } = -1;


    public NewtonianSystemObject(float gravitationalConstant, string name, Game appCtx) : base(name, appCtx)
    {
        GravitationalConstant = gravitationalConstant;
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        Simulation.RegisterSystemRigidBodies();
    }

    public override void OnLoad(GameObject? parentObject)
    {
        Simulation = new NewtonianGravity2DControllerComponent(GravitationalConstant, "Simulation", this.app);
        this.AddComponent(Simulation);
    }

    // expose method to be called when all children are set
}

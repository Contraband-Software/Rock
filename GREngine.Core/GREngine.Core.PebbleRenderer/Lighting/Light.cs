using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GREngine.Core.System;
using static GREngine.Debug.Out;


namespace GREngine.Core.PebbleRenderer;

public class Light : Behaviour
{
    public Vector2 position;
    public Vector3 color;
    private Vector3 coneEdges;// t1 & t2 are the angles of the egdes of the light cone
    private float rotation = 0;
    public bool isShadowCasting;

    public Light(Vector3 color,bool isShadowCasting = false ,float rotation = 0,float arc = 1) { //anything else?
        this.position = new Vector2(0);
        this.color = color;
        this.rotation = rotation;
        this.isShadowCasting = isShadowCasting;
        this.coneEdges.Z = arc;
        calculateThetas();

    }

    protected override void OnStart()
    {
        Game.Services.GetService<IPebbleRendererService>().addLight(this);
    }

    protected override void OnUpdate(GameTime gameTime)
    {
        this.position.X = this.Node.GetGlobalPosition().X;
        this.position.Y = this.Node.GetGlobalPosition().Y;

        //Out.Debug.WriteLine(this.Node.GetGlobalPosition().ToString());
    }

    public void setRotation(float rotation)
    {
        this.rotation = rotation;
        calculateThetas();
    }

    public float getRotation()
    {
        return rotation;
    }

    public void setArc(float arc)
    {
        coneEdges.Z = arc;
    }

    public Vector3 getLightDir()
    {
        return coneEdges;
    }

    private void calculateThetas()// do I want to handle spotlights and pointlights as the same thing or create separate lighting shaders for each as a slight optimisation?
    {
        coneEdges.X = (float)Math.Sin(rotation);
        coneEdges.Y = (float)Math.Cos(rotation);
    }

    protected override void OnDestroy()
    {
        Game.Services.GetService<IPebbleRendererService>().removeLight(this);
    }

}


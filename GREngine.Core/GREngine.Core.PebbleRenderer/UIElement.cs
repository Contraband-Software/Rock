#nullable enable


using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using static System.Net.Mime.MediaTypeNames;

using GREngine.Core.System;

namespace GREngine.Core.PebbleRenderer;

using Debug;

public class UIElement : Behaviour
{
    private String text;
    private Color color;
    private float scale;
    private SpriteFont font;
    private UIDrawable drawable;
    public UIElement(string text,SpriteFont font, Color color, float scale)
    {
        this.text = text;
        this.color = color;
        this.scale = scale;
        this.font = font;

        drawable = new UIDrawable(new Vector2(Node.GetGlobalPosition().X, Node.GetGlobalPosition().X), scale,color, font, text);
    }


    protected override void OnStart()
    {
        //base.OnStart();
        //Out.PrintLn(this.InstanceID.ToString());
    }

    protected override void OnUpdate(GameTime gameTime)
    {
        Game.Services.GetService<IPebbleRendererService>().DrawUI(drawable);
    }






    protected override void OnDestroy()
    {
        //Game.Services.GetService<IPebbleRendererService>().removeSprite(this);
    }



}


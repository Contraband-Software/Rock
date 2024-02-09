using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GREngine.Core.PebbleRenderer;

public class Shader
{
    public Dictionary<string,float> attributes;
    public Effect shader;

    public Shader(Effect shader)
    {
        this.shader = shader;
        attributes = new Dictionary<string, float>();
    }
    public void SetAllParams()
    {
        foreach (KeyValuePair<string, float> atribute in attributes)
        {
            shader.Parameters[atribute.Key]?.SetValue(atribute.Value);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GREngine.Core.PebbleRenderer;

public class Material
{
    public Shader[] shaders;
    public Material(Shader diffuse, Shader normal, Shader roughness)
    {
        this.shaders = new Shader[] { diffuse, normal, roughness };
    }
}


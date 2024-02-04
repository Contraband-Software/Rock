using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace LD54.Engine.Leviathan;
public class LeviathanShader
{
    public List<float> attributeValues;
    public List<string> attributes;
    private Game game;
    public Effect shader;
    public LeviathanShader(Game game, string path)
    {
        this.game = game;
        shader = game.Content.Load<Effect>(path);
        attributeValues = new List<float>();
        attributes = new List<string>();
    }
    public void SetAllParams()
    {
        for (int i = 0; i < attributes.Count; i++)
        {
            shader.Parameters[attributes[i]]?.SetValue(attributeValues[i]);
        }
    }
    public void UpdateParam(string name, float value)
    {
        attributeValues[attributes.IndexOf(name)] = value;
    }
    public void AddParam(string name, float value)
    {
        attributeValues.Add(value);
        attributes.Add(name);
    }
}

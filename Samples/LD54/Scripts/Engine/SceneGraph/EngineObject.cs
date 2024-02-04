namespace LD54.Engine;

using System.Collections.Generic;
using Microsoft.Xna.Framework;

public abstract class EngineObject
{
    public bool Initialized = false;

    protected readonly Game app;

    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    protected string name;

#pragma warning disable CA1051
    // ReSharper disable once UnusedMember.Global
    public List<string> Tags = new();
#pragma warning restore CA1051

    protected EngineObject(string name, Game appCtx)
    {
        this.name = name;
        this.app = appCtx;
    }

    public string GetName()
    {
        return this.name;
    }

    public abstract void OnLoad(GameObject? parentObject);
    public virtual void Update(GameTime gameTime) { }
    public abstract void OnUnload();
}

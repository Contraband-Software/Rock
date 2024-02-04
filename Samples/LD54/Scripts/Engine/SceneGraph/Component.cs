namespace LD54.Engine;

using System;
using Microsoft.Xna.Framework;

public abstract class Component : EngineObject, IUpdateable
{
    public bool Enabled { get; set; } = true;
    public int UpdateOrder { get; }

    public event EventHandler<EventArgs>? EnabledChanged;
    public event EventHandler<EventArgs>? UpdateOrderChanged;

    protected GameObject gameObject;
    public GameObject ContainingGameObject => this.gameObject;

    protected Component(string name, Game appCtx) : base(name, appCtx) { }
}

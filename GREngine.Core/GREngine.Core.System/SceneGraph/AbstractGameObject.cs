using NotImplementedException = System.NotImplementedException;

namespace GREngine.Core.System;

using global::System.Collections.Generic;

/// <summary>
/// Anything that exists within the game scene
/// </summary>
public abstract class AbstractGameObject
{
    public delegate void EnabledChanged(bool state);
    public event EnabledChanged? EnabledChangedEvent;
    public bool Enabled { get; private set; } = true;

    public string Name { get; internal protected set; } = "Generic Object";
    protected internal readonly HashSet<string> Tags = new();

    public virtual void SetEnabled(bool state)
    {
        Enabled = state;
        this.EnabledChangedEvent?.Invoke(state);
    }

    public bool HasTag(string tag)
    {
        return Tags.Contains(tag);
    }
}

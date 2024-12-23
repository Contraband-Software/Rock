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

    public string Name { get; protected set; } = "Generic Object";
    internal protected readonly HashSet<string> Tags = [];

    // ReSharper disable once MemberCanBeProtected.Global UnusedMemberHierarchy.Global
    public virtual void SetEnabled(bool state)
    {
        Enabled = state;
        this.EnabledChangedEvent?.Invoke(state);
    }

    // ReSharper disable once UnusedMember.Global
    public bool HasTag(string tag)
    {
        return Tags.Contains(tag);
    }
}

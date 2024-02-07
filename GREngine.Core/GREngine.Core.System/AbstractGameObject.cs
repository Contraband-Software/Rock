namespace GREngine.Core.System;

public abstract class AbstractGameObject
{
    public bool Enabled { get; private set; }

    internal void OnEnable() { }
    internal void OnDisable() { }

    public void SetEnabled(bool state)
    {
        Enabled = state;
    }
}

namespace GREngine.Core.System;

using Microsoft.Xna.Framework;

/// <summary>
/// Represents a unit of logic within the scene graph
/// </summary>
// Attributes should be used to add behaviours: typeof(...), alternatively, custom nodes are made by functions, not inheritance
public abstract partial class Behaviour
{
    private static uint instances;

    private ISceneManager sceneManager;

    private int loadOrder;
    internal bool Initialized { get; private set; }

    // ReSharper disable MemberCanBePrivate.Global
    protected readonly uint InstanceId;
    // ReSharper restore MemberCanBePrivate.Global

    internal void Initialize(int lo, SceneManager sm, Game ga)
    {
        this.loadOrder = lo;
        this.sceneManager = sm;
        this.Game = ga;
        this.Initialized = true;
    }

#pragma warning disable CS8618
    protected Behaviour()
    {
        Name = "Behaviour";
        this.InstanceId = ++instances;
    }
#pragma warning restore CS8618

    public int CompareTo(Behaviour? other)
    {
        // If other is not a valid object reference, this instance is greater.
        if (other == null) return 1;

        float thisHash = float.Parse(this.loadOrder.ToString() + '.' + this.InstanceId);
        float otherHash = float.Parse(other.loadOrder.ToString() + '.' + other.InstanceId);

        return thisHash.CompareTo(otherHash);
    }
}

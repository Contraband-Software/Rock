namespace GREngine.Core.System;

using global::System.Collections.Generic;
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

    internal class LoadOrderComparer : Comparer<Behaviour>
    {
        public override int Compare(Behaviour? a, Behaviour? b)
        {
            // If other is not a valid object reference, this instance is greater.
            if (b == null) return 1;

            float thisHash = float.Parse(a.loadOrder.ToString()  + '.' + a.InstanceId);
            float otherHash = float.Parse(b.loadOrder.ToString() + '.' + b.InstanceId);

            return thisHash.CompareTo(otherHash);
        }
    }
}

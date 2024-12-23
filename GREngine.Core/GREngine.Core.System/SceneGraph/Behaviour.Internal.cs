namespace GREngine.Core.System;

using Debug;
using global::System;
using global::System.Buffers.Binary;
using global::System.Collections.Generic;
using global::System.Globalization;
using global::System.Linq;
using Microsoft.Xna.Framework;

/// <summary>
/// Represents a unit of logic within the scene graph
/// </summary>
// Attributes should be used to add behaviours: typeof(...), alternatively, custom nodes are made by functions, not inheritance
public abstract partial class Behaviour
{
    private static uint initializations;

    private ISceneManager sceneManager;

    internal int LoadOrder { get; set; }
    internal bool Initialized { get; private set; }

    // ReSharper disable MemberCanBePrivate.Global
    protected readonly uint InstanceId;
    // ReSharper restore MemberCanBePrivate.Global

    internal void Initialize(int loadOrder, SceneManager sm, Game gm)
    {
        this.LoadOrder = loadOrder;

        this.sceneManager = sm;
        this.Game = gm;
        this.Initialized = true;
    }

#pragma warning disable CS8618
    protected Behaviour()
    {
        Name = "Behaviour";
        this.InstanceId = ++initializations;
    }
#pragma warning restore CS8618

    internal class LoadOrderComparer : Comparer<Behaviour>
    {
        public override int Compare(Behaviour? a, Behaviour? b)
        {
            // If other is not a valid object reference, this instance is greater.
            if (b == null) return 1;
            if (a == null) return -1;

            int loadOrderCmp = a.LoadOrder.CompareTo(b.LoadOrder);
            int instanceOrderCmp = a.InstanceId.CompareTo(b.InstanceId);
            return loadOrderCmp * (int)initializations + instanceOrderCmp;
        }
    }
}

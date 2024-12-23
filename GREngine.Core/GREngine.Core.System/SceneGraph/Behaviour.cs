namespace GREngine.Core.System;

using global::System;
using Microsoft.Xna.Framework;

/// <summary>
/// Represents a unit of logic within the scene graph
/// </summary>
// Attributes should be used to add behaviours: typeof(...), alternatively, custom nodes are made by functions, not inheritance
public abstract class Behaviour : AbstractGameObject, IComparable<Behaviour>
{
    // ReSharper disable InconsistentNaming

    private static uint Instances;

    // ReSharper disable MemberCanBePrivate.Global
    protected readonly uint InstanceID;
    // ReSharper restore MemberCanBePrivate.Global

    // ReSharper restore InconsistentNaming

    internal void Initialize(int lo, SceneManager sm, Game ga)
    {
        this.loadOrder = lo;
        this.sceneManager = sm;
        this.Game = ga;
        this.Initialized = true;
    }

    private ISceneControllerService sceneManager;
    private int loadOrder;
    internal bool Initialized { get; private set; }

    internal protected Game Game { get; internal set; }

    public NodePointer Node { get; internal set; }

#pragma warning disable CS8618
    protected Behaviour()
    {
        Name = "Behaviour";
        this.InstanceID = ++Instances;
    }
#pragma warning restore CS8618

    #region USER_IMPLEMENTATION_API
    // ReSharper disable MemberCanBeProtected.Global UnusedMemberHierarchy.Global UnusedParameter.Global
    internal protected virtual void OnAwake() { Name = "Behaviour"; }
    internal protected virtual void OnStart() { Name = "Behaviour"; }

    internal protected virtual void OnUpdate(GameTime gameTime) { }
    internal protected virtual void OnFixedUpdate(GameTime gameTime) { }

    internal protected virtual void OnDestroy() { }
    // ReSharper restore MemberCanBeProtected.Global UnusedMemberHierarchy.Global UnusedParameter.Global
    #endregion

    public override void SetEnabled(bool state)
    {
        base.SetEnabled(state);
        this.sceneManager.BehaviourEnabledChanged(this, state);
    }

    public int CompareTo(Behaviour? other)
    {
        // If other is not a valid object reference, this instance is greater.
        if (other == null) return 1;

        float thisHash = float.Parse(this.loadOrder.ToString() + '.' + this.InstanceID);
        float otherHash = float.Parse(other.loadOrder.ToString() + '.' + other.InstanceID);

        return thisHash.CompareTo(otherHash);
    }
}

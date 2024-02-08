namespace GREngine.Core.System;

using global::System;
using Microsoft.Xna.Framework;

/// <summary>
/// Represents a unit of logic within the scene graph
/// </summary>
public abstract class Behaviour : AbstractGameObject, IComparable<Behaviour>
{
    internal void Initialize(int lo, SceneManager sm, Game ga)
    {
        this.loadOrder = lo;
        this.sceneManager = sm;
        this.Game = ga;
        this.Initialized = true;
    }

    private SceneManager sceneManager;
    private int loadOrder = 0;
    internal bool Initialized { get; private set; } = false;

    protected Game Game { get; private set; }
    internal protected Node? Node { get; internal set; }

#pragma warning disable CS8618
    protected Behaviour() { Name = "Behaviour"; }
#pragma warning restore CS8618

    #region USER_IMPLEMENTATION_API
    internal protected virtual void OnAwake() { Name = "Behaviour"; }
    internal protected virtual void OnStart() { Name = "Behaviour"; }

    internal protected virtual void OnUpdate(GameTime gameTime) { }
    internal protected virtual void OnFixedUpdate(GameTime gameTime) { }

    internal protected virtual void OnDestroy() { }
    #endregion

    public override void SetEnabled(bool state)
    {
        base.SetEnabled(state);

        ((ISceneControllerService)sceneManager).BehaviourEnabledChanged(this, state);
    }

    public int CompareTo(Behaviour? other)
    {
        // If other is not a valid object reference, this instance is greater.
        if (other == null) return 1;

        return this.loadOrder.CompareTo(other.loadOrder);
    }
}

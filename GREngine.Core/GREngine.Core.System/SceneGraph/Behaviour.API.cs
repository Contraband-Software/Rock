namespace GREngine.Core.System;

using global::System;
using Microsoft.Xna.Framework;

public abstract partial class Behaviour : AbstractGameObject
{
    #region PROPERTIES
    public NodePointer Node { get; internal set; }

    internal protected Game Game { get; internal set; }
    #endregion

    #region PROGRAMMABLE_API
    // ReSharper disable MemberCanBeProtected.Global UnusedMemberHierarchy.Global UnusedParameter.Global
    internal protected virtual void OnAwake() { Name = "Behaviour"; }
    internal protected virtual void OnStart() { Name = "Behaviour"; }

    internal protected virtual void OnUpdate(GameTime gameTime) { }
    internal protected virtual void OnFixedUpdate(GameTime gameTime) { }

    internal protected virtual void OnDestroy() { }
    // ReSharper restore MemberCanBeProtected.Global UnusedMemberHierarchy.Global UnusedParameter.Global
    #endregion

    #region FIXED_FUNCTION_API
    public override void SetEnabled(bool state)
    {
        base.SetEnabled(state);
        this.sceneManager.BehaviourEnabledChanged(this, state);
    }
    #endregion
}

namespace GREngine.Core.System;

public abstract class Behaviour : AbstractGameObject, IBehaviour
{
    internal void OnEnable() { }
    internal abstract void OnDisable();

    void IBehaviour.OnAwake() { }
    void IBehaviour.OnStart() { }

    void IBehaviour.OnFixedUpdate() { }
    void IBehaviour.OnUpdate() { }

    void IBehaviour.OnDestroy() { }
}

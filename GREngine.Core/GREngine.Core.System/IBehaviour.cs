namespace GREngine.Core.System;

public interface IBehaviour
{
    internal void OnAwake();
    internal void OnStart();

    internal void OnFixedUpdate();
    internal void OnUpdate();

    internal void OnDestroy();
}

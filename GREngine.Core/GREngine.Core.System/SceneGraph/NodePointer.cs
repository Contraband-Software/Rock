namespace GREngine.Core.System;

using global::System;

public class NodePointer
{
    private WeakReference<Node> weakReference;

    public GameObjectReference(GameObject gameObject)
    {
        weakReference = new WeakReference<GameObject>(gameObject);
    }

    public GameObject Get()
    {
        GameObject target;
        if (weakReference.TryGetTarget(out target) && target.IsValid)
        {
            return target;
        }
        return null;
    }
}

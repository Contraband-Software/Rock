namespace GREngine.Core.System;

using global::System;
using global::System.Collections.Generic;
using global::System.Linq;
using Microsoft.Xna.Framework;

// so basically, if i make it so that this nodeptr structure are the actual public api on the node obj, the private ones can be on the
// weak ptr-ed object, so i can delete that instead and have all the public api be static and let it operate on an associated struct

// needs to be completely readonly actually
// remove free
// actually free can still be public because its functionality is standalone
public sealed class NodePointer
{
    private readonly WeakReference<Node?> weakReference;

    public void AddTag(string tag) => this.Get().Tags.Add(tag);

    internal NodePointer(Node gameObject)
    {
        this.weakReference = new WeakReference<Node?>(gameObject);
    }

    internal Node? Get()
    {
        return this.weakReference.TryGetTarget(out Node? target) ? target : null;
    }

    /// <summary>
    /// Frees the subtree as well
    /// </summary>
    public void Free()
    {
        Node? obj = this.Get();
        obj?.sceneManager.FreeNode(obj);
        this.weakReference.SetTarget(null);
    }

    #region HELPERS
    public Vector2 GetLocalPosition2D()
    {
        Vector2 pos = new();
        this.GetLocalPosition().Deconstruct(out pos.X, out pos.Y, out _);
        return pos;
    }

    public void SetLocalPosition(float x, float y)
    {
        this.SetLocalPosition(new Vector3(x, y, this.GetLocalPosition().Z));
    }
    public void SetLocalPosition(Vector2 localPosition)
    {
        this.SetLocalPosition(new Vector3(localPosition, 1));
    }
    public void SetLocalPosition(Vector3 localPosition)
    {
        Matrix matrix = this.GetLocalTransform();
        matrix.Translation = localPosition;
        this.SetLocalTransform(matrix);
    }

    public Vector3 GetLocalPosition()
    {
        return this.GetLocalTransform().Translation;
    }

    public Vector3 GetGlobalPosition()
    {
        return this.GetGlobalTransform().Translation;
    }

    public Vector2 GetGlobalPosition2D()
    {
        Vector2 pos = new();
        this.GetGlobalTransform().Translation.Deconstruct(out pos.X, out pos.Y, out _);
        return pos;
    }
    #endregion

    #region TRANSFORM_API
    // ReSharper disable MemberCanBePrivate.Global
    public Matrix GetLocalTransform()
    {
        return this.Get()!.GetLocalTransform();
    }

    public void SetLocalTransform(Matrix matrix)
    {
        this.Get()!.transform.matrix = matrix;
    }

    public Matrix GetGlobalTransform()
    {
        return this.Get()!.GetGlobalTransform();
    }
    #endregion

    #region SCENE_API
    // ReSharper disable UnusedMember.Global
    public NodePointer? GetParent()
    {
        return this.Get()!.GetParent();
    }

    public IEnumerable<NodePointer> GetChildren()
    {
        return this.Get()!.GetChildren();
    }
    #endregion

    #region BEHAVIOUR_API
    public IEnumerable<Behaviour> GetAllBehaviours()
    {
        return this.Get()!.GetAllBehaviours<Behaviour>();
    }

    /// <summary>
    /// This will return the FIRST component of type T
    /// </summary>
    /// <typeparam name="T">A Behaviour</typeparam>
    /// <returns></returns>
    // ReSharper disable once UnusedMember.Global
    public Behaviour? GetBehaviour<T>() where T : Behaviour
    {
        return this.Get()!.GetBehaviour<T>();
    }

    /// <summary>
    /// This will return ALL components of type T
    /// </summary>
    /// <typeparam name="T">A Behaviour</typeparam>
    /// <returns></returns>
    public IEnumerable<Behaviour> GetAllBehaviours<T>() where T : Behaviour
    {
        return this.Get()!.GetAllBehaviours<T>();
    }
    // ReSharper restore UnusedMember.Global
    #endregion
}

namespace GREngine.Core.System;

using global::System;
using global::System.Collections.Generic;
using global::System.Linq;
using Microsoft.Xna.Framework;

public abstract class Node : AbstractGameObject
{
    internal Node? parent;
    private Transform transform;
    private SceneManager sceneManager;

    internal readonly List<Node> children = new();
    internal readonly List<Behaviour> behaviours = new();

    protected Node()
    {
        Name = "Node";
        this.transform.matrix = Matrix.Identity;
    }

    #region TRANSFORM_API
    public Matrix GetLocalTransform()
    {
        return this.transform.matrix;
    }
    public void SetLocalTransform(Matrix matrix)
    {
        this.transform.matrix = matrix;
    }
    public Matrix GetGlobalTransform()
    {
        //                                         |-  Statement null if there is no parent.
        //                                         |                       |-  Null-coalescing operator makes these parenthesis
        //                                         V                       V   evaluate to the identity matrix if the above is null.
        return this.transform.matrix * (this.parent?.GetGlobalTransform() ?? Matrix.Identity);
    }

    #region HELPERS
    public void SetLocalPosition(Vector3 localPosition)
    {
        Matrix matrix = this.GetLocalTransform();
        matrix.Translation = localPosition;
        this.SetLocalTransform(matrix);
    }
    public void SetLocalPosition(Vector2 localPosition)
    {
        this.SetLocalPosition(new Vector3(localPosition, 1));
    }
    public Vector3 GetLocalPosition()
    {
        return this.GetLocalTransform().Translation;
    }
    public Vector3 GetGlobalPosition()
    {
        return this.GetGlobalTransform().Translation;
    }
    #endregion
    #endregion

    #region SCENE_API
    public Node? GetParent()
    {
        return this.parent;
    }
    public IEnumerable<Node> GetChildren()
    {
        return this.children;
    }
    #endregion

    #region BEHAVIOUR_API
    public IEnumerable<Behaviour> GetAllBehaviours()
    {
        return this.behaviours;
    }
    /// <summary>
    /// This will return the FIRST component of type T
    /// </summary>
    /// <typeparam name="T">A Behaviour</typeparam>
    /// <returns></returns>
    // ReSharper disable once UnusedMember.Global
    public Behaviour? GetBehaviour<T>() where T : Behaviour
    {
        return this.behaviours.FirstOrDefault(c => c.GetType() == typeof(T));
    }

    /// <summary>
    /// This will return ALL components of type T
    /// </summary>
    /// <typeparam name="T">A Behaviour</typeparam>
    /// <returns></returns>
    public IEnumerable<Behaviour> GetAllBehaviours<T>() where T : Behaviour
    {
        return this.behaviours.Where(c => c.GetType() == typeof(T)).ToList();
    }
    #endregion

    public override void SetEnabled(bool state)
    {
        base.SetEnabled(state);

        ReadOnlySpan<Behaviour> readOnlyBehaviours = this.behaviours.ToArray();
        ((ISceneControllerService)sceneManager).NodeEnabledChanged(readOnlyBehaviours, state);
    }
}

namespace GREngine.Core.System;

using global::System;
using global::System.Collections.Generic;
using global::System.Linq;
using Microsoft.Xna.Framework;

internal sealed class Node : AbstractGameObject
{
    internal SceneManager sceneManager = null!;

    internal Node parent;

    // should be readonly list of nodepointers
    internal readonly List<Node> children = new();
    internal readonly List<Behaviour> behaviours = new();

    internal Transform transform;

    internal NodePointer AsWeakReference() => new(this);

    internal Node(string name = "Node")
    {
        Name = "Node";
        this.transform.matrix = Matrix.Identity;
    }

    public override void SetEnabled(bool state)
    {
        base.SetEnabled(state);

        ReadOnlySpan<Behaviour> readOnlyBehaviours = this.behaviours.ToArray();
        ((ISceneControllerService)sceneManager).NodeEnabledChanged(readOnlyBehaviours, state);
    }

    #region TRANSFORM_API
    public Matrix GetGlobalTransform()
    {
        //         Null-coalescing operator makes these parenthesis  ------|
        //         evaluate to the identity matrix if the above is null.   V
        return this.transform.matrix * (this.parent?.GetGlobalTransform() ?? Matrix.Identity);
        //                                         A
        //                                         |
        //                                         |-  Statement null if there is no parent.
    }

    public Matrix GetLocalTransform()
    {
        return this.transform.matrix;
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
    internal Behaviour? GetBehaviour<T>() where T : Behaviour
    {
        return this.behaviours.FirstOrDefault(c =>
        {
            for (Type? current = c.GetType(); current != null; current = current.BaseType)
            {
                if (current == typeof(T))
                    return true;
            }
            return false;
        });
    }

    /// <summary>
    /// This will return ALL components of type T
    /// </summary>
    /// <typeparam name="T">A Behaviour</typeparam>
    /// <returns></returns>
    internal IEnumerable<Behaviour> GetAllBehaviours<T>() where T : Behaviour
    {
        return this.behaviours.Where(c => c.GetType() == typeof(T)).ToList();
    }
    // ReSharper restore UnusedMember.Global
    #endregion
}

// ReSharper disable MemberCanBePrivate.Global

using System.Linq;

namespace LD54.Engine;

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

public abstract class GameObject : EngineObject, IUpdateable
{
    #region IUPDATEABLE
    // ReSharper disable once ReplaceAutoPropertyWithComputedProperty
    public bool Enabled { get; } = true;
    public int UpdateOrder { get; }
    public event EventHandler<EventArgs>? EnabledChanged;
    public event EventHandler<EventArgs>? UpdateOrderChanged;
    #endregion

    #region EVENTS
    public delegate void ChildAttached(GameObject gameObject);
    public event ChildAttached ChildAttachedEvent;

    public delegate void ChildRemoved(GameObject gameObject);
    public event ChildRemoved ChildRemovedEvent;

    public delegate void ComponentAttached(Component component);
    public event ComponentAttached ComponentAttachedEvent;
    #endregion

    protected GameObject? parent;
    protected readonly List<GameObject> children = new();

    protected Matrix transform;
    protected float Rotation = 0;
    protected readonly List<Component> components = new();

    // public readonly Game App;

    protected GameObject(string name, Game appCtx) : base(name, appCtx)
    {
        this.transform = Matrix.Identity;

        // App = appCtx;
    }

    public new virtual void Update(GameTime gameTime)
    {
        this.UpdateComponents(gameTime);
    }

    public override void OnUnload()
    {
        // PrintLn("Unloading components for: " + this.GetName());
        this.UnloadComponents();
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

    #region SCENE_GRAPH
    public GameObject? GetParent()
    {
        return this.parent;
    }

    // ReSharper disable once MemberCanBeProtected.Global
    public Matrix GetLocalTransform()
    {
        return this.transform;
    }

    // ReSharper disable once MemberCanBeProtected.Global
    public void SetLocalTransform(Matrix transform)
    {
        this.transform = transform;
    }

    public Matrix GetGlobalTransform()
    {
        //                                  |-  Statement null if there is no parent.
        //                                  |                       |-  Null-coalescing operator makes these parenthesis
        //                                  V                       V   evaluate to the identity matrix if the above is null.
        return this.transform * (this.parent?.GetGlobalTransform() ?? Matrix.Identity);
    }

    /// <summary>
    /// Parents the a child to this gameObject.
    /// </summary>
    /// <param name="gameObject"></param>
    public void AddChild(GameObject gameObject)
    {
        gameObject.parent?.RemoveChild(gameObject);
        gameObject.parent = this;
        this.children.Add(gameObject);

        if (ChildAttachedEvent is not null) ChildAttachedEvent(gameObject);
    }

    public IEnumerable<GameObject> GetChildren()
    {
        return this.children;
    }

    public void RemoveChild(GameObject gameObject)
    {
        this.children.Remove(gameObject);
        gameObject.parent = null;

        if (ChildRemovedEvent is not null) ChildRemovedEvent(gameObject);
    }

    public void ClearChildren()
    {
        foreach (GameObject child in this.children)
        {
            child.parent = null;
        }
        this.children.Clear();
        GC.Collect();
    }
    #endregion

    #region COMPONENT_SYSTEM
    // ReSharper disable once MemberCanBeProtected.Global
    public void AddComponent(Component component)
    {
        this.components.Add(component);
        component.OnLoad(this);

        if (ComponentAttachedEvent is not null) ComponentAttachedEvent(component);
    }

    public void RemoveComponent(Component component)
    {
        component.OnUnload();
        this.components.Remove(component);
    }

    protected void UpdateComponents(GameTime gameTime)
    {
        foreach (Component c in this.components.Where(c => c.Enabled))
        {
            c.Update(gameTime);
        }
    }

    protected void UnloadComponents()
    {
        foreach (Component c in this.components.Where(c => c.Enabled).ToList())
        {
            //PrintLn(c.GetType().Name + ": " + c.GetName());
            c.OnUnload();
            c.Enabled = false;
        }
    }

    /// <summary>
    /// This will return the FIRST component of type T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    // ReSharper disable once UnusedMember.Global
    public Component? GetComponent<T>() where T : Component
    {
        return this.components.FirstOrDefault(c => c.GetType() == typeof(T));
    }

    /// <summary>
    /// This will return ALL components of type T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public IEnumerable<Component> GetAllComponents<T>() where T : Component
    {
        return this.components.Where(c => c.GetType() == typeof(T)).ToList();
    }

    public Component GetComponent<T>(string componentName) where T : Component
    {
        IEnumerable<Component> typeComponents = this.GetAllComponents<T>();
        foreach (Component c in typeComponents.Where(c => c.GetName() == componentName))
        {
            return c;
        }

        throw new ArgumentException("No component with that name on this GameObject");
    }

    public IEnumerable<Component> GetAllComponents()
    {
        return this.components;
    }
    #endregion
}

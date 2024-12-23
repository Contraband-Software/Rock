namespace GREngine.Core.System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

public abstract class Scene
{
    protected Game Game { get; private set; } = null!;

    public string Name { get; private set; }

    internal protected ContentManager ContentManager { get; private set; } = null!;

    protected Scene(string name)
    {
        Name = name;
    }

    internal void Initialize(Game game)
    {
        this.Game = game;
        this.ContentManager = new ContentManager(this.Game.Services);
        this.ContentManager.RootDirectory = this.Game.Content.RootDirectory;
    }

    #region USER_IMPLEMENTATION_API
    // ReSharper disable VirtualMemberNeverOverridden.Global
    internal protected virtual void OnLoad(SceneManager sceneManager) { }
    internal protected virtual void OnUnload() { }
    // ReSharper restore VirtualMemberNeverOverridden.Global
    #endregion
}

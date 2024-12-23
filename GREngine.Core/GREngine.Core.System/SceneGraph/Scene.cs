namespace GREngine.Core.System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

public abstract class Scene(string name)
{
    protected Game Game { get; private set; } = null!;

    public string Name { get; private set; } = name;

    internal protected ContentManager ContentManager { get; private set; } = null!;

    internal void Initialize(Game game)
    {
        this.Game = game;
        this.ContentManager = new ContentManager(this.Game.Services);
        this.ContentManager.RootDirectory = this.Game.Content.RootDirectory;
    }

    // this needs to be serialized (XML maybe, or yaml, or toml)
    #region PROGRAMMABLE_API
    // ReSharper disable VirtualMemberNeverOverridden.Global
    internal protected virtual void OnLoad(SceneManager sceneManager) { }
    internal protected virtual void OnUnload() { }
    // ReSharper restore VirtualMemberNeverOverridden.Global
    #endregion
}

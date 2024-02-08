namespace GREngine.Core.System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

public abstract class Scene
{
    protected Game Game { get; private set; }

    public string Name { get; private set; }

    internal protected ContentManager contentManager { get; private set; }

    public Scene(string name)
    {
        Name = name;
    }

    internal void Initialize(Game game)
    {
        this.Game = game;
        this.contentManager = new ContentManager(this.Game.Services);
        this.contentManager.RootDirectory = this.Game.Content.RootDirectory;
    }

    #region USER_IMPLEMENTATION_API
    internal protected virtual void OnLoad(Node rootNode, Node persistantNode) { }
    internal protected virtual void OnUnload() { }
    #endregion
}

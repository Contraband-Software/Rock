namespace LD54.Engine;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

public abstract class Scene : EngineObject
{
    protected readonly ContentManager contentManager;
    public ContentManager ContentManager => contentManager;

    protected Scene(string name, Game appCtx) : base(name, appCtx)
    {
        this.contentManager = new ContentManager(this.app.Services);
        this.contentManager.RootDirectory = this.app.Content.RootDirectory;
    }

    /// <summary>
    /// Releases all game resources in this scene.
    /// </summary>
    /// <param name="permanent">If the scene need not be loaded again. (frees more memory)</param>
    protected void UnloadContent(bool permanent=false)
    {
        this.contentManager.Unload();
        if (permanent) this.contentManager.Dispose();
    }
}

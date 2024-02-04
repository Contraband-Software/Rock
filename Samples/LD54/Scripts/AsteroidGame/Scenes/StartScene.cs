namespace LD54.AsteroidGame.Scenes;

using System.Collections.Generic;
using Engine.Leviathan;
using Microsoft.Xna.Framework;
using Engine;
using GameObjects;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

public class StartScene : Scene
{
    public enum START_SFX : int
    {
        INTRO1 = 0,
        INTRO2 = 1,
        INTRO3 = 2,
        COUNT
    }

    private readonly int showTime = 0;

    private ILeviathanEngineService? render;
    private ISceneControllerService? scene;

    private SoundEffect[] startSceneSFX = new SoundEffect[(int)START_SFX.COUNT];

    public StartScene(int showTime, Game appCtx) : base("StartScene", appCtx)
    {
        this.showTime = showTime;
    }

    public override void OnLoad(GameObject? parentObject)
    {
        render = this.app.Services.GetService<ILeviathanEngineService>();
        scene = this.app.Services.GetService<ISceneControllerService>();

        startSceneSFX[(int)START_SFX.INTRO1] = this.contentManager.Load<SoundEffect>("Sound/SFX/Intro1");
        startSceneSFX[(int)START_SFX.INTRO2] = this.contentManager.Load<SoundEffect>("Sound/SFX/Intro2");
        startSceneSFX[(int)START_SFX.INTRO3] = this.contentManager.Load<SoundEffect>("Sound/SFX/Intro3");

        GameObject titleUI = new TitleScreenSystem(
            this.showTime,
            this.startSceneSFX,
            this.contentManager.Load<SpriteFont>("Fonts/TitleFont"),
            this.contentManager.Load<SpriteFont>("Fonts/SubtitleFont"),
            this.contentManager.Load<Texture2D>("Sprites/rockIcon"),
            this.app
            );

        parentObject.AddChild(titleUI);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }

    public override void OnUnload()
    {

    }
}

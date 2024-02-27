namespace GameDemo1.Scripts;

using System;
using GREngine.Algorithms;
using GREngine.Core.PebbleRenderer;
using GREngine.Core.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

public class DeathSceneController : Behaviour
{

    public DeathSceneController()
    {

    }

    protected override void OnUpdate(GameTime gameTime)
    {
        KeyboardState state = Keyboard.GetState();

        if (state.IsKeyDown(Keys.R))
        {
            this.Game.Services.GetService<ISceneControllerService>().ChangeScene("GameScene");
        }

    }
}


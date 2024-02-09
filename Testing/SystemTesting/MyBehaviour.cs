namespace Testing.SystemTesting;

using GREngine.Core.System;
using GREngine.Debug;
using Microsoft.Xna.Framework;

public class MyBehaviour : Behaviour
{
    private bool done = false;

    protected override void OnAwake()
    {
        base.OnAwake();
        Out.PrintLn("MyBehave: OnAwake");
    }
    protected override void OnStart()
    {
        base.OnStart();
        Out.PrintLn("MyBehave: OnStart");
    }
    protected override void OnUpdate(GameTime gameTime)
    {
        base.OnUpdate(gameTime);
        if (!done)
        {
            Out.PrintLn("MyBehave: OnUpdate");
            done = true;
        }
    }
    protected override void OnFixedUpdate(GameTime gameTime)
    {
        base.OnFixedUpdate(gameTime);
        Out.PrintLn("MyBehave: OnFixedUpdate");
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        Out.PrintLn("MyBehave: OnDestroy");
    }
}

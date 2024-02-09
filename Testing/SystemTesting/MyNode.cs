namespace Testing.SystemTesting;

using GREngine.Core.System;

[GRETagWith("DestroyOnGameOver")]
public class MyNode : Node
{
    public MyNode()
    {
        this.Tags.Add("hello");
    }
}

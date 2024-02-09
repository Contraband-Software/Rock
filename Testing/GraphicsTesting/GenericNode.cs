namespace Testing.SystemTesting;

using GREngine.Core.System;

[GRETagWith("DestroyOnGameOver")]
public class GenericNode : Node
{
    public GenericNode()
    {
        this.Tags.Add("generic");
    }
}

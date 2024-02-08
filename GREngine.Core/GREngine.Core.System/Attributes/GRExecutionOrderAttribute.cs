namespace GREngine.Core.System;

using global::System;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class GRExecutionOrderAttribute : Attribute
{
    public int LoadOrder { get; private set; } = 0;
    public GRExecutionOrderAttribute(int order)
    {
        LoadOrder = order;
    }
}

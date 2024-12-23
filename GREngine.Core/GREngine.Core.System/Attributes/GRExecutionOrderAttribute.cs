namespace GREngine.Core.System;

using global::System;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
// ReSharper disable once InconsistentNaming
public class GRExecutionOrderAttribute : Attribute
{
    public int LoadOrder { get; private set; }
    public GRExecutionOrderAttribute(int order)
    {
        LoadOrder = order;
    }
}

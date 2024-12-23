namespace GREngine.Core.System;

using global::System;

// ReSharper disable once InconsistentNaming
[AttributeUsage(AttributeTargets.All)]
public class GRETagWithAttribute : Attribute
{
    public string[] Tags { get; private set; }

    public GRETagWithAttribute(params string[] tags)
    {
        Tags = tags;
    }
}

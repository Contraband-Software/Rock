namespace GREngine.Core.System;

using global::System;

public class GRETagWithAttribute : Attribute
{
    public string[] Tags { get; private set; }

    public GRETagWithAttribute(params string[] tags)
    {
        Tags = tags;
    }
}

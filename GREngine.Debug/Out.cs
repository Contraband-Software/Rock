namespace GREngine.Debug;

using System;
using System.Diagnostics;

public static class Out
{
    public static void PrintLn(string text)
    {
#if LINUX
        Console.Out.WriteLine(text);
#else
        Debug.WriteLine(text);
#endif
    }
}

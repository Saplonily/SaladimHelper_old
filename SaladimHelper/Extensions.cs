using System;
using System.Collections.Generic;

namespace Celeste.Mod.SaladimHelper.Extensions;

public static class Extensions
{
    public static void Foreach<T>(this IEnumerable<T> things, Action<T> action)
    {
        foreach(var item in things)
        {
            action(item);
        }
    }

    public static T Cast<T>(this object obj)
    {
        return (T)obj;
    }
}

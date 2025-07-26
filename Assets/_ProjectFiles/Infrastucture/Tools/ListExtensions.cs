using System;
using System.Collections.Generic;

public static class ListExtensions
{
    private static readonly Random _rng = new();

    public static IList<T> Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;

        while (n > 1)
        {
            int k = _rng.Next(n--);
            (list[n], list[k]) = (list[k], list[n]);
        }

        return list;
    }
}
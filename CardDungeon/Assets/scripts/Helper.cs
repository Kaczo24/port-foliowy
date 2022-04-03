//using System;
using System.Collections.Generic;
using UnityEngine;


public static class Helper
{
    public static List<T> Randomize<T>(List<T> l)
    {
        List<T> o = new List<T>();
        while (l.Count > 0)
        {
            int i = Random.Range(0, l.Count);
            o.Add(l[i]);
            l.RemoveAt(i);
        }
        return o;
    }
    public static T Pick<T>(IList<T> l) => l[Random.Range(0, l.Count)];
    public static T Pick<T>()
    {
        var v = System.Enum.GetValues(typeof(T));
        return (T)v.GetValue(Random.Range(0, v.Length));
    }
}


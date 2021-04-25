using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ListExtensions
{
    public static bool Available<V>(this List<V> list)
    {
        if (list != null && list.Count > 0)
            return true;

        return false;
    }

    public static bool empty<V>(this List<V> list) { return list.Count == 0; }
    public static bool Available<V>(this V[] array)
    {
        if (array != null && array.Length > 0)
            return true;

        return false;
    }
    public static T[] Insert<T>(this T[] arr, T ele)
    {
        Array.Resize(ref arr, arr.Length + 1);
        arr[arr.Length - 1] = ele;
        return arr;
    }

    public static void Remove<T>(this T[] arr, Predicate<T> predicate)
    {
        arr = Array.FindAll(arr, predicate);
    }

    public static T[] RemoveAt<T>(this T[] source, int index)
    {
        T[] dest = new T[source.Length - 1];
        if (index > 0)
            Array.Copy(source, 0, dest, 0, index);

        if (index < source.Length - 1)
            Array.Copy(source, index + 1, dest, index, source.Length - index - 1);

        return dest;
    }
    public static List<V> DontExsist<V>(this List<V> list, List<V> listCheck)
    {
        if (list.Available() == false)
            if (listCheck.Available())
                return listCheck;

        if (listCheck.Available() == false)
            return new List<V>();

        return listCheck.FindAll(x => list.Contains(x) == false);
    }

}

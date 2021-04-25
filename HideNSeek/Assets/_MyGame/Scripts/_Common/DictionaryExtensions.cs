using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DictionaryExtensions
{
    // True is new
    public static bool TryAdd<K, V>(this Dictionary<K, V> dic, K key, V val)
    {
        if (dic.ContainsKey(key))
        {
            dic[key] = val;
            return false;
        }
        else
        {
            dic.Add(key, val);
            return true;
        }
    }

    public static bool Available<K, V>(this Dictionary<K, V> dic)
    {
        if (dic != null)
        {
            if (dic.Count > 0)
            {
                return true;
            }
        }

        return false;
    }

    public static void TryAdd<K, V>(this Dictionary<K, List<V>> dic, K key, V val)
    {
        if (dic == null)
            dic = new Dictionary<K, List<V>>();

        if (dic.ContainsKey(key))
        {
            if (dic[key] == null)
                dic[key] = new List<V>();

            dic[key].Add(val);
        }
        else
            dic.Add(key, new List<V>() { val });
    }
}

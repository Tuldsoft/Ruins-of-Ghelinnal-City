using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A compilation of tools. Currently, no tools are in use.
/// </summary>
public static class CustomUtils
{
    // A generic that attempts a cast. Better solutions were found and this was never used.
    public static bool TryCast<T>(this object obj, out T result)
    {
        if (obj is T)
        {
            result = (T)obj;
            return true;
        }

        result = default(T);
        return false;
    }
}

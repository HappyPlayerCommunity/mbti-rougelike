using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Tag
{
    public static string Player = "Player";
    public static string Enemy = "Enemy";
    public static string DamageCollider = "DamageCollider";
}

/// <summary>
/// 辅助使用Unity中各种Tag的类。
/// </summary>
public struct TagHelper
{
    public static bool CompareTag(Collider2D hit, params string[] s)
    {
        foreach (var tag in s)
            if (hit.CompareTag(tag))
                return true;

        return false;
    }

    public static bool CompareTag(RaycastHit2D hit, params string[] s)
    {
        foreach (var tag in s)
            if (hit.collider.CompareTag(tag))
                return true;

        return false;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Tag
{
    public const string Player = "Player";
    public const string Enemy = "Enemy";
    public const string Bond = "Bond";
    public const string DamageCollider = "DamageCollider";
    public const string ISTP_Robot = "ISTP_Robot";
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
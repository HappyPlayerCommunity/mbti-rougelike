using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 状态类。状态本身的核心属性只有持续时间和记时。具体的效果都由子类实现。
/// </summary>
public abstract class Status : ScriptableObject
{
    [Tooltip("状态的持续时间。")]
    public float duration;

    [Tooltip("状态的持续时间计时。")]
    public float timer;

    [Tooltip("此状态是否会让玩家禁足。")]
    public bool root = false;

    [Tooltip("此状态是否会禁止玩家充能。")]
    public bool ultChargeBan = false;

    public virtual void OnApply(GameObject target)
    {
        timer = duration;
    }

    public virtual void OnUpdate(GameObject target, float deltaTime)
    {
        timer -= deltaTime;
    }

    public virtual void OnExpire(GameObject target)
    {
    }

    public bool IsExpired()
    {
        return timer <= 0.0f;
    }
}

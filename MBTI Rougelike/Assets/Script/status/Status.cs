using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

/// <summary>
/// 状态类。状态本身的核心属性只有持续时间和记时。具体的效果都由子类实现。
/// </summary>
[System.Serializable]
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

    [Tooltip("此状态是否会进入无敌状态。")]
    public bool invincible = false;

    [Tooltip("此状态是否会禁止玩家释放技能。")]
    public bool silence = false;

    [Tooltip("此状态的强度变化率。")]
    public float modifyPowerRate = 1.0f;

    [Tooltip("此状态的伤害/治疗变化率。")]
    public float modifyImpactRate = 1.0f;

    [Tooltip("此状态的持续时间变化率。")]
    public float modifyDurationRate = 1.0f;

    [Tooltip("此状态的持续时间变化率。")]
    public bool lockDuration = false;

    [Tooltip("此状态的表现动画。")]
    public AnimationController2D statusAnimPrefab;

    protected AnimationController2D recordAnim;

    bool bindingWithSpecialSkill = false;

    [Header("互动组件")]
    public Stats stats;

    public virtual void OnApply(GameObject target)
    {
        if (lockDuration)
            timer = duration;
        else
            timer = duration * modifyDurationRate;
    }

    public virtual void OnUpdate(GameObject target, float deltaTime)
    {
        if (bindingWithSpecialSkill && !Input.GetMouseButton(0))
        {
            OnExpire(target);
        }
        else
        {
            timer -= deltaTime;
        }
    }

    public virtual void OnExpire(GameObject target)
    {
    }

    public virtual void OnStack(Status status)
    {
        duration = Mathf.Max(duration, status.duration);
        timer = duration;

        modifyPowerRate = Mathf.Max(modifyPowerRate, status.modifyPowerRate);
        modifyImpactRate = Mathf.Max(modifyImpactRate, status.modifyImpactRate);
        modifyDurationRate = Mathf.Max(modifyDurationRate, status.modifyDurationRate);
    }

    public bool IsExpired()
    {
        return timer <= 0.0f && !bindingWithSpecialSkill;
    }

    protected void PlayAnimation(GameObject target)
    {
        if (statusAnimPrefab)
        {
            GameObject statusEffect = PoolManager.Instance.GetObject(statusAnimPrefab.name, statusAnimPrefab.gameObject);
            recordAnim = statusEffect.GetComponent<AnimationController2D>();
            recordAnim.attachedTransform = target.transform;
            recordAnim.Activate(target.transform.position, Quaternion.identity);
        }
    }
}

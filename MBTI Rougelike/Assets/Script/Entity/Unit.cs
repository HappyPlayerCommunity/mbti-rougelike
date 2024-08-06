using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 单位类。大部分动来动去的东西都可以归类于此。
/// </summary>
public class Unit : BaseEntity
{
    [SerializeField, Tooltip("此单位的攻击范围。")]
    protected float attackRange = 1.0f;

    [SerializeField, Tooltip("此单位的攻击计时器。低于0才可以进行攻击。")]
    protected float attackTimer = 0.0f;

    [SerializeField, Tooltip("此单位多久可以进行一次攻击。")]
    protected float attackTime = 0.0f;

    [SerializeField, Tooltip("此单位多久可以进行一次攻击。")]
    protected float castTime = 0.0f;

    [SerializeField, Tooltip("此单位可以生成的【伤害块】。")]
    protected DamageCollider damageCollider;

    [SerializeField, Tooltip("此单位生成【伤害块】的点，与自身的距离。")]
    protected float attackInitDistance;

    [SerializeField, Tooltip("此单位生成【伤害块】的速度。")]
    protected float initAttackMovementSpeed;

    [SerializeField, Tooltip("此单位是否正在释放某项技能。")]
    protected bool isActioning;

    [SerializeField, Tooltip("此单位能闪避伤害块的概率。")]
    protected float dodgeRate;

    [SerializeField, Tooltip("此单位的状态管理器，用来结算各种状态。")]
    protected StatusManager statusManager;

    //或许应该攻击技能动作剥离出去，单开一个类。

    public float DodgeRate
    {
        get
        {
            return dodgeRate;
        }
        set
        {
            dodgeRate = value;
        }
    }

    public StatusManager StatusManager
    {
        get
        {
            return statusManager;
        }
    }

    protected override void Start()
    {
        base.Start();
        statusManager = GetComponent<StatusManager>();
    }

    public bool IsActioning
    {
        get { return isActioning; }
        set { isActioning = value; }
    }

    public override bool CanTakeDamageFrom(GameObject collider)
    {
        return base.CanTakeDamageFrom(collider) && !statusManager.IsInvincible();
    }
}

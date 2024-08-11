using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 存放角色的能力值。Still Working on it...
/// </summary>
[CreateAssetMenu(fileName = "NewStats", menuName = "Stats Data")]
public class Stats : ScriptableObject
{
    [Header("E")]
    [Tooltip("火属性伤害。")]
    public float fireDamage;
    [Tooltip("移动速度。")]
    public float movementSpeed;
    [Tooltip("韧性")]
    public float toughness;
    [Tooltip("羁绊移动速度")]
    public float bondMovementSpeed;
    [Tooltip("攻击范围")]
    public float attackRange;
    [Tooltip("攻击充能率")]
    public float attackEnergeCharge;


    [Header("I")]
    [Tooltip("冰属性伤害")]
    public float iceDamage;
    [Tooltip("护盾上限")]
    public float maxShield;
    [Tooltip("护盾重置速率")]
    public float shieldReset;
    [Tooltip("建筑耐久")]
    public float buildingDurability;
    [Tooltip("击退")]
    public float knockback;
    [Tooltip("受伤充能率")]
    public float injuryEnergeCharge;


    [Header("S")]
    [Tooltip("地属性伤害")]
    public float earthDamage;
    [Tooltip("生命上限")]
    public float maxHealth;
    [Tooltip("生命再生")]
    public float healthRegen;
    [Tooltip("经验获取率")]
    public float experienceMultiplier;
    [Tooltip("攻击速度")]
    public float attackSpeed;
    [Tooltip("实体攻击力")]
    public float physicalAttackPower;


    [Header("N")]
    [Tooltip("风属性伤害")]
    public float windDamage;
    [Tooltip("闪避率")]
    public float dodge;
    [Tooltip("自动充能率")]
    public float autoEnergeCharge;
    [Tooltip("幸运")]
    public float luck;
    [Tooltip("特技冷却")]
    public float specialCooldown;
    [Tooltip("抽象攻击力")]
    public float abstractAttackPower;


    [Header("T")]
    [Tooltip("雷属性攻击")]
    public float thunderDamage;
    [Tooltip("全局攻击力")]
    public float globalAttackPower;


    [Header("F")]
    [Tooltip("水属性攻击")]
    public float waterDamage;
    [Tooltip("治疗力")]
    public float healingPower;


    [Header("J")]
    [Tooltip("暴击率")]
    public float crit;
    [Tooltip("暴击伤害")]
    public float critDamage;
    [Tooltip("建筑强度")]
    public float buildingPower;
    [Tooltip("建筑强度")]
    public float bondPower;


    [Header("P")]
    [Tooltip("状态强度")]
    public float statusPower;
    [Tooltip("状态持续时间")]
    public float statusDuration;
    [Tooltip("状态造成的伤害/治疗")]
    public float statusImpact;
    [Tooltip("状态(异常)抗性")]
    public float anomalyResistance;


    [Header("初始值")]
    public float fireDamageINIT = 0.0f;
    public float movementSpeedINIT = 0.0f;
    public float toughnessINIT = 0.0f;
    public float bondMovementSpeedINIT = 0.0f;
    public float attackRangeINIT = 0.0f;           // hmm
    public float attackEnergeChargeINIT = 0.0f;
    public float iceDamageINIT = 0.0f;
    public float maxShieldINIT = 0.0f;
    public float shieldResetINIT = 0.0f;
    public float buildingDurabilityINIT = 0.0f;
    public float knockbackINIT = 0.0f;
    public float injuryEnergeChargeINIT = 0.0f;
    public float earthDamageINIT = 0.0f;
    public float maxHealthINIT = 0.0f;
    public float healthRegenINIT = 0.0f;
    public float experienceMultiplierINIT = 0.0f;
    public float attackSpeedINIT = 0.0f;
    public float physicalAttackPowerINIT = 0.0f;
    public float windDamageINIT = 0.0f;
    public float dodgeINIT = 0.0f;
    public float autoEnergeChargeINIT = 0.0f;
    public float luckINIT = 0.0f;
    public float specialCooldownINIT = 1.0f;
    public float abstractAttackPowerINIT = 0.0f;
    public float thunderDamageINIT = 0.0f;
    public float globalAttackPowerINIT = 0.0f;
    public float waterDamageINIT = 0.0f;
    public float healingPowerINIT = 0.0f;
    public float critINIT = 0.0f;
    public float critDamageINIT = 0.0f;
    public float buildingPowerINIT = 0.0f;
    public float bondPowerINIT = 0.0f;
    public float statusPowerINIT = 0.0f;
    public float statusDurationINIT = 0.0f;
    public float statusImpactINIT = 0.0f;
    public float anomalyResistanceINIT = 0.0f;

    // 【测试用】用于存储上一次的数值，用于判断是否发生了变化。
    private float previousMaxHealth = 0.0f;
    private float previousHealthRegen = 0.0f;
    private float previousMaxShield = 0.0f;
    private float previousShieldReset = 0.0f;
    private float previousDodgeRate = 0.0f;
    private float previousPhysicalAttackPower = 0.0f;
    private float previousAbstractAttackPower = 0.0f;
    private float previousGlobalAttackPower = 0.0f;
    private float previousToughness = 0.0f;
    private float previousCrit = 0.0f;
    private float previousCritDamageRate = 0.0f;

    public const float percentage = 0.01f;

    private float ApplyPercentageMultiplier(float value)
    {
        return 1.0f + value * percentage;
    }

    private float ApplyPercentage(float value)
    {
        return value * percentage;
    }

    // 用于通过编辑器调数值时，更新相关的能力数据。
    public delegate void ValidateEvent();
    public event ValidateEvent OnValueChange;

    // 下面这个部分还在施工中，有许多属性需要后续系统完善后才能实现。

    /// <summary>
    /// 【初步实现】
    /// </summary>
    public float Calculate_FireDamage()
    {
        return ApplyPercentageMultiplier(fireDamage);
    }

    public const float basicMovementSpeed = 5.0f;

    /// <summary>
    /// 【初步实现】
    /// </summary>
    public float Calculate_MovementSpeed()
    {
        return basicMovementSpeed * ApplyPercentageMultiplier(movementSpeed);
    }

    /// <summary>
    /// 【初步实现】
    /// </summary>
    public float Calculate_Toughness()
    {
        return 1.0f / ApplyPercentageMultiplier(toughness);
    }

    /// <summary>
    /// 【未实现】羁绊系统还未实现。
    /// </summary>
    public float Calculate_BondMovementSpeed()
    {
        return bondMovementSpeed;
    }

    /// <summary>
    /// 【初步实现】
    /// </summary>
    public float Calculate_AttackRange()
    {
        // 对两类伤害块的有不同的加成。
        // 静态：每1点增加1%的碰撞体积；
        // 动态：每1点增加1%的持续时间，变相增加了射程。
        return ApplyPercentageMultiplier(attackRange);
    }

    /// <summary>
    /// 【初步实现】
    /// </summary>
    public float Calculate_AttackEnergeCharge()
    {
        return ApplyPercentageMultiplier(attackEnergeCharge);
    }

    /// <summary>
    /// 【初步实现】
    /// </summary>
    public float Calculate_IceDamage()
    {
        return ApplyPercentageMultiplier(iceDamage);

    }

    /// <summary>
    /// 【初步实现】
    /// </summary>
    public float Calculate_MaxShield()
    {
        return maxShield;
    }

    public const float basicShieldReset = 10.0f;

    /// <summary>
    /// 【初步实现】
    /// </summary>
    public float Calculate_ShieldReset()
    {
        return 1.0f / ApplyPercentageMultiplier(shieldReset);

    }

    /// <summary>
    /// 【未实现】建筑系统未实现。
    /// </summary>
    public float Calculate_BuildingDurability()
    {
        return buildingDurability;
    }
    
    /// <summary>
    /// 【初步实现】
    /// </summary>
    public float Calculate_Knockback()
    {
        return ApplyPercentageMultiplier(knockback);
    }

    /// <summary>
    /// 【初步实现】
    /// </summary>
    public float Calculate_InjuryEnergeCharge()
    {
        return ApplyPercentageMultiplier(injuryEnergeCharge);
    }

    /// <summary>
    /// 【初步实现】
    /// </summary>
    public float Calculate_EarthDamage()
    {
        return ApplyPercentageMultiplier(earthDamage);

    }

    public const float basicMaxHealth = 100.0f;
    /// <summary>
    /// 【初步实现】
    /// </summary>
    public float Calculate_MaxHealth()
    {
        return basicMaxHealth * ApplyPercentageMultiplier(maxHealth);
    }

    /// <summary>
    /// 【初步实现】目前采用的常数值。
    /// </summary>
    public float Calculate_HealthRegen()
    {
        return healthRegen;
    }

    /// <summary>
    /// 【未实现】经验系统未实现。
    /// </summary>
    public float Calculate_ExperienceMultiplier()
    {
        return experienceMultiplier;
    }

    /// <summary>
    /// 【初步实现】
    /// </summary>
    public float Calculate_AttackSpeed()
    {
        return 1.0f / ApplyPercentageMultiplier(attackSpeed);
    }

    /// <summary>
    /// 【初步实现】
    /// </summary>
    public float Calculate_PhysicalAttackPower()
    {
        return ApplyPercentageMultiplier(physicalAttackPower);
    }

    /// <summary>
    /// 【初步实现】
    /// </summary>
    public float Calculate_WindDamage()
    {
        return ApplyPercentageMultiplier(windDamage);

    }

    /// <summary>
    /// 【初步实现】动画的击中效果，闪避的动画效果还未实现。视觉上比较怪异。
    /// </summary>
    public float Calculate_Dodge()
    {
        return ApplyPercentage(Mathf.Clamp(dodge, 0.0f, 100.0f));
    }

    /// <summary>
    /// 【初步实现】
    /// </summary>
    public float Calculate_AutoCharge()
    {
        return autoEnergeCharge;
    }

    /// <summary>
    /// 【未实现】还未实现天赋/装备系统。
    /// </summary>
    public float Calculate_Luck()
    {
        return luck;
    }

    /// <summary>
    /// 【初步实现】
    /// </summary>
    public float Calculate_SpecialCooldown()
    {
        return 1.0f / ApplyPercentageMultiplier(specialCooldown);
    }

    /// <summary>
    /// 【初步实现】
    /// </summary>
    public float Calculate_AbstractAttackPower()
    {
        return ApplyPercentageMultiplier(abstractAttackPower);
    }

    /// <summary>
    /// 【初步实现】
    /// </summary>
    public float Calculate_ThunderDamage()
    {
        return ApplyPercentageMultiplier(thunderDamage);

    }

    /// <summary>
    /// 【初步实现】
    /// </summary>
    public float Calculate_GlobalAttackPower()
    {
        return ApplyPercentageMultiplier(globalAttackPower);
    }

    /// <summary>
    /// 【初步实现】
    /// </summary>
    public float Calculate_WaterDamage()
    {
        return ApplyPercentageMultiplier(waterDamage);
    }

    /// <summary>
    /// 【未实现】治疗公式和系统还未实现。
    /// </summary>
    public float Calculate_HealingPower()
    {
        return healingPower;
    }

    /// <summary>
    /// 【未实现】伤害公式还未实现。
    /// </summary>
    public float Calculate_Crit()
    {
        return ApplyPercentage(Mathf.Clamp(crit, 0.0f, 100.0f));
    }

    /// <summary>
    /// 【未实现】伤害公式还未实现。
    /// </summary>
    public float Calculate_CritDamageRate()
    {
        return ApplyPercentageMultiplier(critDamage);
    }

    /// <summary>
    /// 【未实现】建筑系统还未实现。
    /// </summary>
    public float Calculate_BuildingPower()
    {
        return buildingPower;
    }

    /// <summary>
    /// 【未实现】羁绊系统还未实现。
    /// </summary>
    public float Calculate_BondPower()
    {
        return bondPower;
    }

    /// <summary>
    /// 【初步实现】
    /// </summary>
    /// <returns></returns>
    public float Calculate_StatusPower()
    {
        return ApplyPercentageMultiplier(statusPower);
    }

    /// <summary>
    /// 【初步实现】
    /// </summary>
    public float Calculate_StatusDuration()
    {
        return ApplyPercentageMultiplier(statusDuration);
    }

    /// <summary>
    /// 【未实现】还没有造成伤害/治疗的buff。
    /// </summary>
    public float Calculate_StatusImpact()
    {
        return ApplyPercentageMultiplier(statusImpact);
    }

    /// <summary>
    /// 【未实现】还没有会施加debuff的敌人。
    /// </summary>
    public float Calculate_AnomalyResistance
    {
        get { return anomalyResistance; }
        set { anomalyResistance = value; }
    }

    /// <summary>
    /// 初始化方法，重置所有能力值为初始值
    /// </summary>
    public void Initialize()
    {
        fireDamage = fireDamageINIT;
        movementSpeed = movementSpeedINIT;
        toughness = toughnessINIT;
        bondMovementSpeed = bondMovementSpeedINIT;
        attackRange = attackRangeINIT;
        attackEnergeCharge = attackEnergeChargeINIT;
        iceDamage = iceDamageINIT;
        maxShield = maxShieldINIT;
        shieldReset = shieldResetINIT;
        buildingDurability = buildingDurabilityINIT;
        knockback = knockbackINIT;
        injuryEnergeCharge = injuryEnergeChargeINIT;
        earthDamage = earthDamageINIT;
        maxHealth = maxHealthINIT;
        healthRegen = healthRegenINIT;
        experienceMultiplier = experienceMultiplierINIT;
        attackSpeed = attackSpeedINIT;
        physicalAttackPower = physicalAttackPowerINIT;
        windDamage = windDamageINIT;
        dodge = dodgeINIT;
        autoEnergeCharge = autoEnergeChargeINIT;
        luck = luckINIT;
        specialCooldown = specialCooldownINIT;
        abstractAttackPower = abstractAttackPowerINIT;
        thunderDamage = thunderDamageINIT;
        globalAttackPower = globalAttackPowerINIT;
        waterDamage = waterDamageINIT;
        healingPower = healingPowerINIT;
        crit = critINIT;
        critDamage = critDamageINIT;
        buildingPower = buildingPowerINIT;
        bondPower = bondPowerINIT;
        statusPower = statusPowerINIT;
        statusDuration = statusDurationINIT;
        statusImpact = statusImpactINIT;
        anomalyResistance = anomalyResistanceINIT;
    }

    /// <summary>
    /// 有一些数值中转存储于实体上，在编辑器上调增数值时，需要通过这里来更新。
    /// </summary>
    public void OnValidate()
    {
        bool changed = false;
        if (maxHealth != previousMaxHealth)
        {
            previousMaxHealth = maxHealth;
            changed = true;
        }

        if (healthRegen != previousHealthRegen)
        {
            previousHealthRegen = healthRegen;
            changed = true;
        }

        if (maxShield != previousMaxShield)
        {
            previousMaxShield = maxShield;
            changed = true;
        }

        if (shieldReset != previousShieldReset)
        {
            previousShieldReset = shieldReset;
            changed = true;
        }

        if (dodge != previousDodgeRate)
        {
            previousDodgeRate = dodge;
            changed = true;
        }


        if (physicalAttackPower != previousPhysicalAttackPower)
        {
            previousPhysicalAttackPower = physicalAttackPower;
            changed = true;
        }

        if (abstractAttackPower != previousAbstractAttackPower)
        {
            previousAbstractAttackPower = abstractAttackPower;
            changed = true;
        }

        if (globalAttackPower != previousGlobalAttackPower)
        {
            previousGlobalAttackPower = globalAttackPower;
            changed = true;
        }

        if (toughness != previousToughness)
        {
            previousToughness = toughness;
            changed = true;
        }

        if (crit != previousCrit)
        {
            previousCrit = crit;
            changed = true;
        }

        if (critDamage != previousCritDamageRate)
        {
            previousCritDamageRate = critDamage;
            changed = true;
        }

        if (changed && OnValueChange != null)
        {
            OnValueChange.Invoke();
        }
    }
}

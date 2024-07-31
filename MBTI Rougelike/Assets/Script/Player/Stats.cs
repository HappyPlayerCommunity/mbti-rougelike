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
    [Tooltip("护盾再生")]
    public float shieldRegen;
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
    public float autoCharge;
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
    public float shieldRegenINIT = 0.0f;
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
    public float autoChargeINIT = 0.0f;
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
    public float buffPowerINIT = 0.0f;
    public float buffDurationINIT = 0.0f;
    public float buffImpactINIT = 0.0f;
    public float anomalyResistanceINIT = 0.0f;

    private float previousMaxHealth = 0.0f;

    public const float percentage = 0.01f;

    private float percentageMultiplierCast(float value)
    {
        return 1.0f + value * percentage;
    }

    // 用于通过编辑器调数值时，更新相关的能力数据。
    public delegate void ValidateEvent();
    public event ValidateEvent OnValueChange;

    // 下面这个部分还在施工中，有许多属性需要后续系统完善后才能实现。

    /// <summary>
    /// 【施工中】元素属性还未实现。
    /// </summary>
    public float Calculate_FireDamage()
    {
        return percentageMultiplierCast(fireDamage);
    }

    public const float basicMovementSpeed = 5;

    /// <summary>
    /// 【初步实现】
    /// </summary>
    public float Calculate_MovementSpeed()
    {
        return basicMovementSpeed * percentageMultiplierCast(movementSpeed);
    }

    /// <summary>
    /// 【施工中】韧性系统还未实现。
    /// </summary>
    public float Calculate_Toughness()
    {
        return toughness;
    }

    /// <summary>
    /// 【施工中】羁绊系统还未实现。
    /// </summary>
    public float Calculate_BondMovementSpeed()
    {
        return bondMovementSpeed;
    }

    public float Calculate_AttackRange() //感觉要思考一下，吃这个加成的攻击方式可能比想象的少。
    {
        // 对两类伤害块的有不同的加成。
        // 静态：每1点增加1%的碰撞体积；
        // 动态：每1点增加1%的持续时间，变相增加了射程。
        return percentageMultiplierCast(attackRange);
    }

    public float Calculate_AttackEnergeCharge()
    {
        return percentageMultiplierCast(attackEnergeCharge);
    }

    public float Calculate_IceDamage()
    {
        return iceDamage;
    }

    public float Calculate_MaxShield()
    {
        return maxShield;
    }

    public float Calculate_ShieldRegen()
    {
        return shieldRegen;
    }

    public float Calculate_BuildingDurability()
    {
        return buildingDurability;
    }

    public float Calculate_Knockback()
    {
        return knockback;
    }

    public float Calculate_InjuryEnergeCharge()
    {
        return percentageMultiplierCast(injuryEnergeCharge);
    }

    public float Calculate_EarthDamage()
    {
        return earthDamage;
    }

    public const float basicMaxHealth = 100.0f;
    public float Calculate_MaxHealth()
    {
        return basicMaxHealth * percentageMultiplierCast(maxHealth);
    }

    public float Calculate_HealthRegen()
    {
        return percentageMultiplierCast(healthRegen);
    }

    public float Calculate_ExperienceMultiplier()
    {
        return experienceMultiplier;
    }

    public float Calculate_AttackSpeed()
    {
        return 1.0f / percentageMultiplierCast(attackSpeed);
    }

    public float Calculate_PhysicalAttackPower()
    {
        return physicalAttackPower;
    }

    public float Calculate_WindDamage()
    {
        return windDamage;
    }

    public float Calculate_Dodge()
    {
        return dodge;
    }

    public float Calculate_AutoCharge()
    {
        return autoCharge;
    }

    public float Calculate_Luck()
    {
        return luck;
    }

    public float Calculate_SpecialCooldown()
    {
        return specialCooldown;
    }

    public float Calculate_AbstractAttackPower()
    {
        return abstractAttackPower;
    }

    public float Calculate_ThunderDamage()
    {
        return thunderDamage;
    }

    public float Calculate_GlobalAttackPower()
    {
        return globalAttackPower;
    }

    public float Calculate_WaterDamage()
    {
        return waterDamage;
    }

    public float Calculate_HealingPower()
    {
        return healingPower;
    }

    public float Calculate_Crit()
    {
        return crit;
    }

    public float Calculate_CritDamage()
    {
        return critDamage;
    }

    public float Calculate_BuildingPower()
    {
        return buildingPower;
    }

    public float Calculate_BondPower()
    {
        return bondPower;
    }

    public float Calculate_StatusPower()
    {
        return statusPower;
    }

    public float Calculate_StatusDuration()
    {
        return statusDuration;
    }

    public float Calculate_StatusImpact()
    {
        return statusImpact;
    }

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
        shieldRegen = shieldRegenINIT;
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
        autoCharge = autoChargeINIT;
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
        statusPower = buffPowerINIT;
        statusDuration = buffDurationINIT;
        statusImpact = buffImpactINIT;
        anomalyResistance = anomalyResistanceINIT;
    }

    public void OnValidate()
    {
        if (maxHealth != previousMaxHealth)
        {
            previousMaxHealth = maxHealth;
            if (OnValueChange != null)
            {
                OnValueChange.Invoke();
            }
        }
    }
}

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
    public float injuryCharge;


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
    public float buffPower;
    [Tooltip("状态持续时间")]
    public float buffDuration;
    [Tooltip("状态造成的伤害/治疗")]
    public float buffImpact;
    [Tooltip("状态(异常)抗性")]
    public float anomalyResistance;



    [Header("初始值")]
    public const float fireDamageINIT = 100.0f;
    public const float movementSpeedINIT = 100.0f;
    public const float toughnessINIT = 0.0f;
    public const float bondMovementSpeedINIT = 0.0f;
    public const float attackRangeINIT = 0.0f;           // hmm
    public const float attackEnergeChargeINIT = 100.0f;
    public const float iceDamageINIT = 0.0f;
    public const float maxShieldINIT = 0.0f;
    public const float shieldRegenINIT = 0.0f;
    public const float buildingDurabilityINIT = 0.0f;
    public const float knockbackINIT = 0.0f;
    public const float injuryChargeINIT = 0.0f;
    public const float earthDamageINIT = 0.0f;
    public const float maxHealthINIT = 0.0f;
    public const float healthRegenINIT = 0.0f;
    public const float experienceMultiplierINIT = 0.0f;
    public const float attackSpeedINIT = 0.0f;
    public const float physicalAttackPowerINIT = 0.0f;
    public const float windDamageINIT = 0.0f;
    public const float dodgeINIT = 0.0f;
    public const float autoChargeINIT = 0.0f;
    public const float luckINIT = 0.0f;
    public const float specialCooldownINIT = 1.0f;
    public const float abstractAttackPowerINIT = 0.0f;
    public const float thunderDamageINIT = 0.0f;
    public const float globalAttackPowerINIT = 0.0f;
    public const float waterDamageINIT = 0.0f;
    public const float healingPowerINIT = 0.0f;
    public const float critINIT = 0.0f;
    public const float critDamageINIT = 0.0f;
    public const float buildingPowerINIT = 0.0f;
    public const float bondPowerINIT = 0.0f;
    public const float buffPowerINIT = 0.0f;
    public const float buffDurationINIT = 0.0f;
    public const float buffImpactINIT = 0.0f;
    public const float anomalyResistanceINIT = 0.0f;

    public const float percentage = 0.01f;

    private float percentageMultiplierCast(float value)
    {
        return 1.0f + value * percentage;
    }

    public float Calculate_FireDamage()
    {
        return percentageMultiplierCast(fireDamage);
    }

    public const float basicMovementSpeed = 5;

    public float Calculate_MovementSpeed()
    {
        return basicMovementSpeed * percentageMultiplierCast(movementSpeed);
    }

    public float Toughness
    {
        get { return toughness; }
        set { toughness = value; }
    }

    public float BondMovementSpeed
    {
        get { return bondMovementSpeed; }
        set { bondMovementSpeed = value; }
    }

    public float AttackRange
    {
        get { return attackRange; }
        set { attackRange = value; }
    }

    public float AttackEnergeCharge
    {
        get { return attackEnergeCharge; }
        set { attackEnergeCharge = value; }
    }

    public float IceDamage
    {
        get { return iceDamage; }
        set { iceDamage = value; }
    }

    public float MaxShield
    {
        get { return maxShield; }
        set { maxShield = value; }
    }

    public float ShieldRegen
    {
        get { return shieldRegen; }
        set { shieldRegen = value; }
    }

    public float BuildingDurability
    {
        get { return buildingDurability; }
        set { buildingDurability = value; }
    }

    public float Knockback
    {
        get { return knockback; }
        set { knockback = value; }
    }

    public float InjuryCharge
    {
        get { return injuryCharge; }
        set { injuryCharge = value; }
    }

    public float EarthDamage
    {
        get { return earthDamage; }
        set { earthDamage = value; }
    }

    public float MaxHealth
    {
        get { return maxHealth; }
        set { maxHealth = value; }
    }

    public float HealthRegen
    {
        get { return healthRegen; }
        set { healthRegen = value; }
    }

    public float ExperienceMultiplier
    {
        get { return experienceMultiplier; }
        set { experienceMultiplier = value; }
    }

    public float Calculate_AttackSpeed()
    {
        return 1.0f / percentageMultiplierCast(attackSpeed);
    }

    public float PhysicalAttackPower
    {
        get { return physicalAttackPower; }
        set { physicalAttackPower = value; }
    }

    public float WindDamage
    {
        get { return windDamage; }
        set { windDamage = value; }
    }

    public float Dodge
    {
        get { return dodge; }
        set { dodge = value; }
    }

    public float AutoCharge
    {
        get { return autoCharge; }
        set { autoCharge = value; }
    }

    public float Luck
    {
        get { return luck; }
        set { luck = value; }
    }

    public float SpecialCooldown
    {
        get { return specialCooldown; }
        set { specialCooldown = value; }
    }

    public float AbstractAttackPower
    {
        get { return abstractAttackPower; }
        set { abstractAttackPower = value; }
    }

    public float ThunderDamage
    {
        get { return thunderDamage; }
        set { thunderDamage = value; }
    }

    public float GlobalAttackPower
    {
        get { return globalAttackPower; }
        set { globalAttackPower = value; }
    }

    public float WaterDamage
    {
        get { return waterDamage; }
        set { waterDamage = value; }
    }

    public float HealingPower
    {
        get { return healingPower; }
        set { healingPower = value; }
    }

    public float Crit
    {
        get { return crit; }
        set { crit = value; }
    }

    public float CritDamage
    {
        get { return critDamage; }
        set { critDamage = value; }
    }

    public float BuildingPower
    {
        get { return buildingPower; }
        set { buildingPower = value; }
    }

    public float BondPower
    {
        get { return bondPower; }
        set { bondPower = value; }
    }

    public float BuffPower
    {
        get { return buffPower; }
        set { buffPower = value; }
    }

    public float BuffDuration
    {
        get { return buffDuration; }
        set { buffDuration = value; }
    }

    public float BuffImpact
    {
        get { return buffImpact; }
        set { buffImpact = value; }
    }

    public float AnomalyResistance
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
        injuryCharge = injuryChargeINIT;
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
        buffPower = buffPowerINIT;
        buffDuration = buffDurationINIT;
        buffImpact = buffImpactINIT;
        anomalyResistance = anomalyResistanceINIT;
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Stats;

/// <summary>
/// 存放角色的人格倾向值。这些数值决定了玩家的人格模型，以及其他能力值的基础值。
/// </summary>
[CreateAssetMenu(fileName = "NewPreferenceData", menuName = "Preference Data")]
public class Preference : ScriptableObject
{
    [Tooltip("【外倾】：与火，移速，韧性，羁绊移速，攻击范围，攻击充能率有关。")]
    public int Extraversion = 0;

    [Tooltip("【内倾】：与冰，护盾上限，护盾恢复率，建筑耐久，击退，受伤充能率有关。")]
    public int Introversion = 0;

    [Tooltip("【实感】：与地，生命上限，生命再生，经验倍率，攻速，实体攻击力有关。")]
    public int Sensing = 0;

    [Tooltip("【直觉】：与风，闪避率，自动充能率，幸运，特技冷却，抽象攻击力有关。")]
    public int Intuition = 0;

    [Tooltip("【思维】：与雷，全局攻击力有关。")]
    public int Thinking = 0;

    [Tooltip("【情感】：与水，治疗力有关。")]
    public int Feeling = 0;

    [Tooltip("【决断】：与暴击，暴击伤害，建筑强度，和羁绊强度有关。")]
    public int Judging = 0;

    [Tooltip("【展望】：与异常强度，异常持续时间，异常伤害，异常抗性有关。")]
    public int Perceiving = 0;

    public delegate void ValidateEvent();
    public event ValidateEvent OnValueChange;

    public void SetUpStats(Stats stats)
    {
        // [E] Extraversion
        stats.fireDamage = Extraversion;
        stats.movementSpeed = Extraversion;
        stats.toughness = Extraversion;
        stats.bondMovementSpeed = Extraversion;
        stats.attackRange = Extraversion;
        stats.attackEnergeCharge = Extraversion;

        // [I] Introversion
        stats.iceDamage = Introversion;
        stats.maxShield = Introversion;
        stats.shieldReset = Introversion;
        stats.buildingDurability = Introversion;
        stats.knockback = Introversion;
        stats.injuryEnergeCharge = Introversion;

        // [S] Sensing
        stats.earthDamage = Sensing;
        stats.maxHealth = Sensing;
        stats.healthRegen = Sensing;
        stats.experienceMultiplier = Sensing;
        stats.attackSpeed = Sensing;
        stats.physicalAttackPower = Sensing;

        // [N] Intuition
        stats.windDamage = Intuition;
        stats.dodge = Intuition * 0.5f;
        stats.autoEnergeCharge = Intuition;
        stats.luck = Intuition;
        stats.specialCooldown = Intuition;
        stats.abstractAttackPower = Intuition;

        // [T] Thinking
        stats.thunderDamage = Thinking;
        stats.globalAttackPower = Thinking * 2;

        // [F] Feeling
        stats.waterDamage = Feeling;
        stats.healingPower = Feeling * 2;

        // [J] Judging
        stats.crit = Judging;
        stats.critDamage = Judging;
        stats.buildingPower = Judging;
        stats.bondPower = Judging;

        // [P] Perceiving
        stats.statusPower = Perceiving;
        stats.statusDuration = Perceiving;
        stats.statusImpact = Perceiving;
        stats.anomalyResistance = Perceiving;
    }

    public void OnValidate()
    {
        if (OnValueChange != null)
        {
            OnValueChange.Invoke();
        }
    }
}
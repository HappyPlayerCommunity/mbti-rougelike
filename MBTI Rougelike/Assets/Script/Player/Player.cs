using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家类。继承自单位，用来实现一些独属于玩家的功能。
/// </summary>
public class Player : Unit
{
    [Header("互动组件")]

    [Tooltip("玩家Art的Transform。")]
    public Transform playerArtTransform;

    [Tooltip("武器Art的Transform。")]
    public Transform weaponArtTransform;

    [Tooltip("武器Art的Transform。")]
    public Personality personality;

    [Tooltip("人格八维数据。")]
    public Preference preference;

    [Tooltip("能力值数据")]
    public Stats stats;

    protected override void Start()
    {
        base.Start();
        stats.Initialize();

        StatsUpdate();

        hp = maxHp;
        shield = maxShield;

        stats.OnValueChange += () => StatsUpdate();
    }

    public override void TakeDamage(int damage, float stuntime)
    {
        base.TakeDamage(damage, stuntime);
        float boostCharge = stats.Calculate_InjuryEnergeCharge(); 
        personality.InjuryChargeEnerge(damage, boostCharge); // 受伤充能比率还得具体设计。
    }

    private void StatsUpdate()
    {
        maxHp = Mathf.RoundToInt(stats.Calculate_MaxHealth());
        hpRegen = Mathf.RoundToInt(stats.Calculate_HealthRegen());

        maxShield = Mathf.RoundToInt(stats.Calculate_MaxShield());

        shieldReset = stats.Calculate_ShieldReset();

        dodgeRate = stats.Calculate_Dodge();

        physicalAtkPower = stats.Calculate_PhysicalAttackPower();
        abstractAtkPower = stats.Calculate_AbstractAttackPower();
        globalAtkPower = stats.Calculate_GlobalAttackPower();
    }
}

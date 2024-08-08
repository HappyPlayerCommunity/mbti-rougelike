using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.ShaderKeywordFilter.FilterAttribute;

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

        preference.SetUpStats(stats);

        StatsUpdate();
        preference.OnValueChange += () => SetUpPreferenceToStats();
        stats.OnValueChange += () => StatsUpdate();

        hp = maxHp;
        shield = maxShield;
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

        toughness = stats.Calculate_Toughness();

        crit = stats.Calculate_Crit();
        critDamageRate = stats.Calculate_CritDamageRate();
    }

    public override float GetElementDamageRate(DamageElementType damageElementType)
    {
        switch (damageElementType)
        {
            case DamageElementType.None:
                return 1.0f;
            case DamageElementType.Fire:
                Debug.Log("Fire");
                return stats.Calculate_FireDamage();
            case DamageElementType.Ice:
                Debug.Log("Ice");
                return stats.Calculate_IceDamage();
            case DamageElementType.Earth:
                Debug.Log("Earth");
                return stats.Calculate_EarthDamage();
            case DamageElementType.Wind:
                Debug.Log("Wind");
                return stats.Calculate_WindDamage();
            case DamageElementType.Thunder:
                Debug.Log("Thunder");
                return stats.Calculate_ThunderDamage();
            case DamageElementType.Water:
                Debug.Log("Water");
                return stats.Calculate_WaterDamage();
            default:
                return 1.0f;
        }
    }

    public void SetUpPreferenceToStats()
    {
        preference.SetUpStats(stats);
        StatsUpdate();
    }
}

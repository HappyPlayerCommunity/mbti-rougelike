using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 这个类会计算和转换Preference和Stats，以供其他地方读取。
/// </summary>
public class StatsManager : MonoBehaviour
{
    public Stats stats;
    public Preference preference;

    public void ApplyModifier(Stats stats, Preference preference)
    {

    }

    public void ApplyExtraversion(Stats stats, Preference preference)
    {
        int E = preference.Extraversion;
        stats.fireDamage = E;
        stats.movementSpeed = E;
        stats.toughness = E;
        stats.bondMovementSpeed = E;
        stats.attackRange = E;
        stats.attackEnergeCharge = E;
    }

}

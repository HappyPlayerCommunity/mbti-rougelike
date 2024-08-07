using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageManager
{
    private static Dictionary<GameObject, HashSet<BaseEntity>> damageRecords = new Dictionary<GameObject, HashSet<BaseEntity>>();

    public static int CalculateDamage(DamageType damageType, int damage, BaseEntity owner)
    {
        float returnDamage = damage; // int -> float

        switch (damageType)
        {
            case DamageType.Physical:
                returnDamage *= owner.PhysicalAtkPower * owner.GlobalAtkPower;
                break;
            case DamageType.Abstract:
                returnDamage *= owner.AbstractAtkPower * owner.GlobalAtkPower;
                break;
            default:
                break;
        }

        return (int)returnDamage;
    }

    public static void RegisterDamage(GameObject damageCollider, BaseEntity target)
    {
        if (!damageRecords.ContainsKey(damageCollider))
        {
            damageRecords[damageCollider] = new HashSet<BaseEntity>();
        }
        damageRecords[damageCollider].Add(target);
    }

    public static void ClearReferences(GameObject damageCollider)
    {
        if (damageRecords.ContainsKey(damageCollider))
        {
            foreach (var target in damageRecords[damageCollider])
            {
                target.ClearDamageTimer(damageCollider);
            }
            damageRecords.Remove(damageCollider);
        }
    }
}
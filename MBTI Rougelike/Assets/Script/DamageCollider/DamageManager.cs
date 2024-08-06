using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageManager
{
    private static Dictionary<GameObject, HashSet<BaseEntity>> damageRecords = new Dictionary<GameObject, HashSet<BaseEntity>>();

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
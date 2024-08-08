using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageManager
{
    const float basicCritDamage = 2.0f;

    private static Dictionary<GameObject, HashSet<BaseEntity>> damageRecords = new Dictionary<GameObject, HashSet<BaseEntity>>();

    public static int CalculateDamage(DamageType damageType, int damage, BaseEntity owner, ref bool isCrit, DamageElementType element)
    {
        float returnDamage = damage; // int -> float

        // 临时tips：对象池生成一些火属性伤害块后，如果直接去修改prefab为其他元素属性，对象池内的伤害块并不会自动更新。
        // 这主要是是一个测试时才会出现的问题。实际运用时，只需要人格类在伤害块激活后，再赋予其属性即可。
        returnDamage *= owner.GetElementDamageRate(element);

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

        isCrit = IsCritical(owner);
        if (isCrit)
        {
            Debug.Log("Critical Hit!");
            returnDamage *= basicCritDamage * owner.CritDamageRate;
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


    public static bool IsCritical(BaseEntity entity)
    {
        if (entity == null)
        {
            return false;
        }

        float randomValue = UnityEngine.Random.value;

        // 如果随机数大于闪避概率，则成功命中
        if (randomValue < entity.Crit)
        {
            return true;
        }

        return false;
    }
}
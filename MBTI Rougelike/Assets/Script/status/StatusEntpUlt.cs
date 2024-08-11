using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewENTPStatus", menuName = "Status Data/ENTP Ult Data")]
public class StatusEntpUlt : Status
{
    public float attackSpeedBonus;
    public float attackRangedBonus;

    public override void OnUpdate(GameObject target, float deltaTime)
    {
        base.OnUpdate(target, deltaTime);
    }

    public override void OnApply(GameObject target)
    {
       base.OnApply(target);
        Debug.Log("Apply?");

        stats.attackSpeed += attackSpeedBonus * modifyPowerRate;
       stats.attackRange += attackRangedBonus * modifyPowerRate;
    }

    public override void OnExpire(GameObject target)
    {
        base.OnExpire(target);

        Debug.Log("Call?");
        stats.attackSpeed -= attackSpeedBonus * modifyPowerRate;
        stats.attackRange -= attackRangedBonus * modifyPowerRate;
    }

}

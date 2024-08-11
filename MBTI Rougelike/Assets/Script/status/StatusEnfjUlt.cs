using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewENFJStatus", menuName = "Status Data/ENFJ Ult Data")]
public class StatusEnfjUlt : Status
{
    public float movementSpeedBonus;

    public override void OnUpdate(GameObject target, float deltaTime)
    {
        base.OnUpdate(target, deltaTime);
    }

    public override void OnApply(GameObject target)
    {
        base.OnApply(target);

        stats.movementSpeed += movementSpeedBonus * modifyPowerRate;
        var player = target.GetComponent<Player>();
        player.StatsUpdate();
    }

    public override void OnExpire(GameObject target)
    {
        base.OnExpire(target);
        stats.movementSpeed -= movementSpeedBonus * modifyPowerRate;
        var player = target.GetComponent<Player>();
        player.StatsUpdate();
    }
}

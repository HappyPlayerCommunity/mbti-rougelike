using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "NewESTPStatus", menuName = "Status Data/ESTP Ult Data")]
public class StatusEstpUlt : Status
{
    public float accelrationBoostRate = 2.0f;
    public float attackSpeedBonus = 2.0f;
    public float skillCooldownBonus = 2.0f;
    private float toungness = 10000.0f;

    public override void OnUpdate(GameObject target, float deltaTime)
    {
        base.OnUpdate(target, deltaTime);
    }

    public override void OnApply(GameObject target)
    {
        base.OnApply(target);
        Player player = target.GetComponent<Player>();
        EstpController esptController = player.GetComponent<EstpController>();

        esptController.ultRocket = true;
        esptController.acceleration *= accelrationBoostRate * modifyPowerRate;
        stats.attackSpeed += attackSpeedBonus * modifyPowerRate;
        stats.specialCooldown += skillCooldownBonus * modifyPowerRate;
        stats.toughness += toungness;
        PlayAnimation(target);

        player.StatsUpdate();
    }

    public override void OnExpire(GameObject target)
    {
        base.OnExpire(target);
        Player player = target.GetComponent<Player>();
        EstpController esptController = player.GetComponent<EstpController>();

        esptController.ultRocket = false;
        esptController.acceleration /= accelrationBoostRate * modifyPowerRate;
        stats.attackSpeed -= attackSpeedBonus * modifyPowerRate;
        stats.specialCooldown -= skillCooldownBonus * modifyPowerRate;
        stats.toughness -= toungness;
        recordAnim.OnAnimationEnd();

        player.StatsUpdate();
    }
}

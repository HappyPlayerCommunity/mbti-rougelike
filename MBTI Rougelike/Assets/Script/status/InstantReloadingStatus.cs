using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

[CreateAssetMenu(fileName = "NewInstantReloadingStatus", menuName = "Status Data/Instant Reloading Data")]
public class InstantReloadingStatus : Status
{
    public float movementSpeedBonus;

    public override void OnUpdate(GameObject target, float deltaTime)
    {
        base.OnUpdate(target, deltaTime);
    }

    public override void OnApply(GameObject target)
    {
        base.OnApply(target);

        var player = target.GetComponent<Player>();

        if (player)
        {
            player.personality.NormalAttack_CurretReloadingTimer = 0.0f;
            player.personality.NormalAttackClip = player.personality.NormalAttack.MaxClip;
        }
    }

    public override void OnExpire(GameObject target)
    {
        base.OnExpire(target);
    }
}

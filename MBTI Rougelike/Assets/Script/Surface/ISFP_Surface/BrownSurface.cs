using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrownSurface : Surface
{
    public float physicalAttackBonus = 30.0f;

    public override void ApplyEffect(GameObject obj)
    {
        if (obj && TagHelper.CompareTag(obj, Tag.Player, Tag.Bond))
        {
            var entity = obj.GetComponent<BaseEntity>();

            if (entity is Player)
            {
                var player = entity as Player;

                player.stats.physicalAttackPower += physicalAttackBonus;
                player.StatsUpdate();
            }
            else
            {
                //队友的攻击加成等羁绊实现后统一结算。
                //entity.MovementSpeed *= Stats.ApplyPercentageMultiplier(movementSpeedBonus);
            }
        }
    }

    public override void RemoveEffect(GameObject obj)
    {
        if (obj && TagHelper.CompareTag(obj, Tag.Player, Tag.Bond))
        {
            var entity = obj.GetComponent<BaseEntity>();

            if (entity is Player)
            {
                var player = entity as Player;

                player.stats.physicalAttackPower -= physicalAttackBonus;
                player.StatsUpdate();
            }
            else
            {
                //队友的攻击加成等羁绊实现后统一结算。
            }
        }
    }
}

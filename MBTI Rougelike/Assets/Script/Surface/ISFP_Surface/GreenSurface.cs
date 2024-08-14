using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenSurface : Surface
{
    public float dodgeBonus = 50.0f;

    public override void ApplyEffect(GameObject obj)
    {
        if (obj && TagHelper.CompareTag(obj, Tag.Player, Tag.Bond))
        {
            var entity = obj.GetComponent<BaseEntity>();

            if (entity is Player)
            {
                var player = entity as Player;

                player.stats.dodge += dodgeBonus;
                player.StatsUpdate();
            }
            else
            {
                entity.MovementSpeed *= Stats.ApplyPercentageMultiplier(dodgeBonus);
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

                player.stats.dodge -= dodgeBonus;
                player.StatsUpdate();
            }
            else
            {
                entity.MovementSpeed /= Stats.ApplyPercentageMultiplier(dodgeBonus);
            }
        }
    }
}

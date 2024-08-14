using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class RedSurface : Surface
{
    public float movementSpeedBonus = 100.0f;

    public override void ApplyEffect(GameObject obj)
    {
        if (obj && TagHelper.CompareTag(obj, Tag.Player, Tag.Bond))
        {
            var entity = obj.GetComponent<BaseEntity>();

            if (entity is Player)
            {
                var player = entity as Player;

                player.stats.movementSpeed += movementSpeedBonus;
                player.StatsUpdate();
            }
            else
            {
                entity.MovementSpeed *= Stats.ApplyPercentageMultiplier(movementSpeedBonus);
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

                player.stats.movementSpeed -= movementSpeedBonus;
                player.StatsUpdate();
            }
            else
            {
                entity.MovementSpeed /= Stats.ApplyPercentageMultiplier(movementSpeedBonus);
            }
        }
    }
}

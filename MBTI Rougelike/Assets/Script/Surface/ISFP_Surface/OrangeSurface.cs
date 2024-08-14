using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrangeSurface : Surface
{
    public float critBonus = 100.0f;

    public override void ApplyEffect(GameObject obj)
    {
        if (obj && TagHelper.CompareTag(obj, Tag.Player, Tag.Bond))
        {
            var entity = obj.GetComponent<BaseEntity>();

            if (entity is Player)
            {
                var player = entity as Player;

                player.stats.crit += critBonus;
                player.StatsUpdate();
            }
            else
            {
                entity.Crit += critBonus;
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

                player.stats.crit -= critBonus;
                player.StatsUpdate();
            }
            else
            {
                entity.Crit -= critBonus;
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretUpgrade : PersonalitySpecialImplementation
{
    public float attackTimeRateUpgrade = 0.0f;

    public override void ExecuteSpecialImplementation(Personality personality)
    {
        // 实现炮塔升级逻辑。后续可以改成升级为全新的炮塔。目前先加一下攻速。
        Turret[] existingTurrets = Object.FindObjectsOfType<Turret>();

        foreach (Turret turret in existingTurrets)
        {
            turret.attackTimeRate *= attackTimeRateUpgrade;
        }
    }
}

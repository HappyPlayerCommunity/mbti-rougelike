using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretDuplication : PersonalitySpecialImplementation
{
    public override void ExecuteSpecialImplementation(Personality personality)
    {
        // 实现炮塔复制逻辑
        Turret[] existingTurrets = Object.FindObjectsOfType<Turret>();

        foreach (Turret turret in existingTurrets)
        {
            Vector3 newTurretPosition = turret.transform.position + new Vector3(turret.transform.localScale.x, 0, 0);
            string poolKey = turret.gameObject.name;
            GameObject newTurretObj = PoolManager.Instance.GetObject(poolKey, turret.gameObject);
            Turret newTurret = newTurretObj.GetComponent<Turret>();
            newTurret.Activate(newTurretPosition, Quaternion.identity);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class AngelRing : DamageCollider
{
    public AngelTurret angelTurret;

    protected override void CollideEvents(Collider2D hit)
    {
        var enemy = hit.GetComponent<Enemy>();
        if (enemy)
        {
            enemy.Die();
            string poolKey = angelTurret.gameObject.name;
            GameObject turretObj = PoolManager.Instance.GetObject(poolKey, angelTurret.gameObject);

            Turret turret = turretObj.GetComponent<Turret>();
            turret.Activate(enemy.transform.position, Quaternion.Euler(0.0f, 0.0f, 0.0f));
        }
    }
}

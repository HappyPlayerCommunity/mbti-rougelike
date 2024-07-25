using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 这种敌人AI在见到玩家后，会不断追击玩家并发起攻击。
/// </summary>
public class MeleeEnemyAI : UnitAI
{
    protected override void Idle()
    {
    }

    protected override void Chase()
    {
        if (Vector3.Distance(transform.position, player.position) < attackRange)
        {
            currentState = State.Attack;
        }
    }

    protected override void Attack()
    {
        if (Vector3.Distance(transform.position, player.position) > attackRange)
        {
            currentState = State.Chase;
        }
    }

    protected override void Retreat() { }
    protected override void Flee() { }
    protected override void Charge() { }
}

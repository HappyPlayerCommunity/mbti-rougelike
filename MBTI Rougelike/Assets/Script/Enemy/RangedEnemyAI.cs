using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 远程敌人AI类。用来管理远程敌人的行为逻辑。
/// </summary>
public class RangedEnemyAI : UnitAI
{
    [SerializeField, Tooltip("逃离玩家的距离。")]
    private float fleeDistance = 5.0f;

    protected override void Idle()
    {
        if (Vector3.Distance(transform.position, playerTrans.position) < detectionRange)
        {
            currentState = State.Chase;
        }
    }

    protected override void Chase()
    {
        if (Vector3.Distance(transform.position, playerTrans.position) < attackRange)
        {
            currentState = State.Attack;
        }
    }

    protected override void Attack()
    {
        if (Vector3.Distance(transform.position, playerTrans.position) > attackRange)
        {
            currentState = State.Chase;
        }
        else if (Vector3.Distance(transform.position, playerTrans.position) < fleeDistance)
        {
            currentState = State.Flee;
        }
    }

    protected override void Retreat()
    {
    }

    protected override void Flee()
    {
        if (Vector3.Distance(transform.position, playerTrans.position) > fleeDistance)
        {
            currentState = State.Attack;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeEnemyAI : UnitAI
{
    protected override void Idle()
    {
        if(unit.AttackTimer <= 0.0f && Vector3.Distance(transform.position, playerTrans.position) < attackRange)
        {
            currentState = State.Attack;
        }
    }

    protected override void Chase()
    {
        
    }

    protected override void Attack()
    {
        if (unit.AttackTimer > 0.0f)
        {
            currentState = State.Idle;
        }
    }

    protected override void Retreat() { }
    protected override void Flee() { }
}

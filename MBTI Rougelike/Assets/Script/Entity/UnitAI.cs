using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 单位AI类。用来管理单位的行为逻辑。我们项目敌人的复杂程度不太高（maybe？），目前使用的是状态机。
/// </summary>
public abstract class UnitAI : MonoBehaviour
{
    public enum State
    {
        Idle,       // 待机，巡逻阶段。
        Chase,      // 发现玩家后，追击玩家
        Attack,     // 攻击玩家
        Retreat,    // 退回“防御点”。
        Flee       // 逃离玩家。
    }

    [SerializeField, Tooltip("当前AI状态。")]
    protected State currentState;

    protected Unit unit;
    protected Transform playerTrans;
    protected Transform currentTarget;

    [SerializeField, Tooltip("检测到玩家的范围。")]
    protected float detectionRange = 10.0f;

    [SerializeField, Tooltip("攻击范围。")]
    protected float attackRange = 10.0f;

    public State CurrentState
    {
        get { return currentState; }
        set { currentState = value; }
    }

    protected virtual void Start()
    {
        var player = GameObject.FindWithTag("Player");
        if (player)
        {
            playerTrans = player.transform;
        }
        unit = GetComponent<Unit>();
    }

    protected virtual void Update()
    {
        if (playerTrans == null) //for now
            return;

        switch (currentState)
        {
            case State.Idle:
                Idle();
                break;

            case State.Chase:
                Chase();
                break;

            case State.Attack:
                Attack();
                break;

            case State.Retreat:
                Retreat();
                break;

            case State.Flee:
                Flee();
                break;
        }
    }

    protected virtual void Idle() { }
    protected virtual void Chase() { }
    protected virtual void Attack() { }
    protected virtual void Retreat() { }
    protected virtual void Flee() { }
}

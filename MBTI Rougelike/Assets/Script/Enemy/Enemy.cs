using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敌人的基类。在这里实现了大部分敌人的共同行为；特殊的敌人可以另开子类实现。
/// </summary>
public class Enemy : Unit
{
    [SerializeField, Tooltip("用于追踪，检测玩家的索引。")]
    private Player player;

    [SerializeField, Tooltip("怪物的艺术层Transform。")]
    private Transform artTransform;

    private UnitAI unitAi;

    public delegate void DeathEvent();
    public event DeathEvent OnEnemyDeath;

    protected override void Start()
    {
        base.Start();
        player = FindObjectOfType<Player>(); // EnemyManager Later.
        unitAi = GetComponent<UnitAI>();
    }

    protected override void OnUpdate()
    {
        if (hp <= 0)
        {
            gameObject.SetActive(false);
        }

        if (player == null) // 临时
            return;

        var distanceVec = player.transform.position - transform.position;

        Vector3 direction = Vector3.Normalize(distanceVec);
        float distance = Vector3.Distance(player.transform.position, transform.position);

        attackTimer -= Time.deltaTime;

        switch (unitAi.CurrentState)
        {
            case UnitAI.State.Idle:
                Idle();
                break;
            case UnitAI.State.Chase:
                Chase();
                break;
            case UnitAI.State.Attack:
                Attack();
                break;
            case UnitAI.State.Retreat:
                break;
            case UnitAI.State.Flee:
                break;
            default:
                break;
        }
    }

    public void Idle()
    {

    }

    public void Chase()
    {
        var distanceVec = player.transform.position - transform.position;
        Vector3 direction = Vector3.Normalize(distanceVec);
        float distance = Vector3.Distance(player.transform.position, transform.position);
        velocity = movementSpeed * direction;
        VelocityUpdate();
    }

    public void Attack()
    {
        var distanceVec = player.transform.position - transform.position;
        Vector3 direction = Vector3.Normalize(distanceVec);
        float distance = Vector3.Distance(player.transform.position, transform.position);

        velocity = Vector3.zero;

        if (distance <= attackRange)
        {
            if (attackTimer <= 0.0f)
            {
                DamageCollider attackBox = Instantiate(damageCollider, transform.position + (distanceVec.normalized * attackInitDistance), Quaternion.Euler(0.0f, 0.0f, 0.0f));
                attackBox.owner = transform.GetComponent<Unit>();

                attackBox.Velocity = (distanceVec.normalized * initAttackMovementSpeed);

                attackTimer = attackTime;
            }
        }
    }

    private void VelocityUpdate()
    {
        if (velocity.x > 0.0f)
        {
            artTransform.localScale = new Vector3(-1.0f, artTransform.localScale.y, 0);
        }
        else if (velocity.x < 0.0f)
        {
            artTransform.localScale = new Vector3(1.0f, artTransform.localScale.y, 0);
        }
    }

    protected override void Die()
    {
        if (OnEnemyDeath != null)
        {
            OnEnemyDeath.Invoke();
        }
        base.Die();
    }
}

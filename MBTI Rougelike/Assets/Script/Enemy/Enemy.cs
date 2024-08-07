using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敌人的基类。在这里实现了大部分敌人的共同行为；特殊的敌人可以另开子类实现。
/// </summary>
public class Enemy : Unit, IPoolable
{
    [SerializeField, Tooltip("用于追踪，检测玩家的索引。")]
    private Player player;

    [SerializeField, Tooltip("怪物的艺术层Transform。")]
    private Transform artTransform;

    private UnitAI unitAi;
    private string poolKey;

    public delegate void DeathEvent();
    public event DeathEvent OnEnemyDeath;

    bool firstTimeCreated = true;

    public string PoolKey
    {
        get { return poolKey; }
        set { poolKey = value; }
    }

    protected override void Awake()
    {
        base.Awake();
        poolKey = gameObject.name;
    }

    protected override void Start()
    {
        base.Start();

        player = FindObjectOfType<Player>(); // EnemyManager Later.
        unitAi = GetComponent<UnitAI>();
    }

    protected override void OnUpdate()
    {
        //if (hp <= 0)
        //{
        //    //gameObject.SetActive(false);
        //    Deactivate();
        //}

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
                string poolKey = damageCollider.name;
                GameObject damageColliderObj = PoolManager.Instance.GetObject(poolKey, damageCollider.gameObject);
                DamageCollider collider = damageColliderObj.GetComponent<DamageCollider>();
                collider.Activate(transform.position + (distanceVec.normalized * attackInitDistance), Quaternion.Euler(0.0f, 0.0f, 0.0f));
                collider.owner = transform.GetComponent<Unit>();

                //DamageCollider attackBox = Instantiate(damageCollider, transform.position + (distanceVec.normalized * attackInitDistance), Quaternion.Euler(0.0f, 0.0f, 0.0f));
                //attackBox.owner = transform.GetComponent<Unit>();

                collider.Velocity = (distanceVec.normalized * initAttackMovementSpeed);

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

    public override void Die()
    {
        if (OnEnemyDeath != null)
        {
            OnEnemyDeath.Invoke();
        }
        Deactivate();
        base.Die();
    }

    /// <summary>
    /// 继承自IPoolable接口的方法。用于对象池物体的初始化。
    /// </summary>
    public void ResetObjectState()
    {
        hp = maxHp;
        shield = maxShield;
        velocity = Vector3.zero;
        blowForceVelocity = Vector3.zero;

        //Debug.Log("Enemy ResetObjectState?");
    }

    /// <summary>
    /// 当对象从对象池中取出时，调用这个方法来初始化
    /// </summary>
    public void Activate(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;

        if (firstTimeCreated)
        {
            firstTimeCreated = false;
        }
        else
        {
            CreateHealthBar();
        }

        gameObject.SetActive(true);
    }

    /// <summary>
    /// 调用这个方法将对象塞回对象池
    /// </summary>
    public void Deactivate()
    {
        gameObject.SetActive(false);

        PoolManager.Instance.ReturnObject(poolKey, gameObject);
    }
}

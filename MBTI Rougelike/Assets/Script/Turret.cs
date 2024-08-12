using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Turret : Building, IPoolable
{
    public float detectionRadius = 5.0f; // 检测半径
    public DamageCollider damageCollider; // 伤害块预制件
    public Transform attackInitPos; // 发射点

    public float attackTimer = 0.0f;
    public float attackTime = 0.0f;

    public float scatterAngle = 10.0f;

    public bool isFixPos = false;

    public Player player;

    public float damageColliderSpeed = 5.0f;
    private string poolKey;

    public float adjustBackOffset = 0.0f;

    public bool mousetGuiding = false;

    Skill.RenderMode damageColliderRenderMode = Skill.RenderMode.NoneFlip;

    [Tooltip("炮塔的攻速，0~1。越低攻速越快。")]
    public float attackTimeRate = 1.0f;

    public string PoolKey
    {
        get { return poolKey; }
        set { poolKey = value; }
    }

    protected override void Start()
    {
        base.Start();
        player = GameObject.FindObjectOfType<Player>();
    }

    protected override void OnUpdate()
    {
        // 更新发射冷却时间
        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0.0f)
        {
            if (mousetGuiding && player.IsAlive() && Input.GetMouseButton(0))
            {
                Vector3 mouseScreenPosition = Input.mousePosition;
                Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
                mouseWorldPosition.z = 0.0f;
                Vector3 direction = (mouseWorldPosition - transform.position).normalized;
                Attack(direction);
            }
            else 
            {
                Collider2D nearestEnemy = FindNearestEnemy();
                if (nearestEnemy != null)
                {
                    Vector3 direction = (nearestEnemy.transform.position - transform.position).normalized;
                    Attack(direction);
                }
            }

            attackTimer = attackTime * attackTimeRate;
        }
    }

    Collider2D FindNearestEnemy()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
        Collider2D nearestEnemy = null;
        float minDistance = Mathf.Infinity;

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                float distance = Vector2.Distance(transform.position, hit.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestEnemy = hit;
                }
            }
        }

        return nearestEnemy;
    }

    void Attack(Vector3 direction)
    {
        if (isFixPos)
        {
            float initDistance = detectionRadius / 2;
            //attackInitPos = transform;
            attackInitPos.position = transform.position + direction * initDistance;
            AttackHelper.InitTurretDamageCollider(damageCollider, attackInitPos, adjustBackOffset, direction, scatterAngle, true, damageColliderRenderMode, player, damageColliderSpeed);
        }
        else
        {
            AttackHelper.InitTurretDamageCollider(damageCollider, attackInitPos, 0.0f, direction, scatterAngle, isFixPos, damageColliderRenderMode, player, damageColliderSpeed);
        }
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

        spriteRenderer.color = Color.white;
        staggerTimer = 0.0f;
        staggerRecordTime = 0.0f;

        //Debug.Log("Enemy ResetObjectState?");
    }

    /// <summary>
    /// 当对象从对象池中取出时，调用这个方法来初始化
    /// </summary>
    public void Activate(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
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

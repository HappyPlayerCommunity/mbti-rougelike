using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : Building
{
    public float detectionRadius = 5.0f; // 检测半径
    public DamageCollider damageCollider; // 伤害块预制件
    public Transform attackInitPos; // 发射点

    public float attackTimer = 0.0f;
    public float attackTime = 0.0f;

    public float scatterAngle = 10.0f;

    bool isFixPos = false;

    public Player player;

    public float damageColliderSpeed = 5.0f;

    Skill.RenderMode damageColliderRenderMode = Skill.RenderMode.NoneFlip;

    protected override void Start()
    {
        base.Start();
        player = GameObject.FindObjectOfType<Player>();
    }

    void Update()
    {
        // 更新发射冷却时间
        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0.0f)
        {
            Collider2D nearestEnemy = FindNearestEnemy();
            if (nearestEnemy != null)
            {
                Vector3 direction = (nearestEnemy.transform.position - transform.position).normalized;
                Attack(direction);
                attackTimer = attackTime;
                //float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                //transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            }
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
        AttackHelper.InitTurretDamageCollider(damageCollider, attackInitPos, direction, scatterAngle, isFixPos, damageColliderRenderMode, player, damageColliderSpeed);
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public enum RobotMode
{
    Melee,
    Ranged
}

public class IstpRobot : BaseEntity
{
    public float meleeRange = 2.0f; // 近战攻击范围
    public float rangedRange = 10.0f; // 远程攻击范围
    public float shootRange = 1.0f; // 远程攻击范围

    public DamageCollider meleeAttack; 
    public DamageCollider rangedAttack;

    public Transform attackInitPos; // 攻击生成点

    public RobotMode currentMode;

    public float attackTimer = 1.0f;
    public float meleeAttackTime = 1.5f;
    public float rangedAttackTime = 0.2f;
    public float rangedScatterAngle = 10.0f;
    public float rangedAttackBulletSpeed = 5.0f;

    private Transform target;
    private Player player;

    public Sprite rangedSprite;
    public Sprite meleeSprite;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        currentMode = RobotMode.Melee;

        player = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        target = FindClosestEnemy();

        attackTimer -= Time.deltaTime;

        if (attackTimer < 0.0f)
        {
            if (target != null)
            {
                Vector3 direction = (target.position - transform.position).normalized;

                if (direction.x < 0.0f)
                {
                    spriteRenderer.transform.localScale = new Vector3(-1.0f, spriteRenderer.transform.localScale.y, 0);
                }
                else if (direction.x > 0.0f) 
                {
                    spriteRenderer.transform.localScale = new Vector3(1.0f, spriteRenderer.transform.localScale.y, 0);
                }

                switch (currentMode)
                {
                    case RobotMode.Melee:
                        MeleeAttack(direction);
                        attackTimer = meleeAttackTime;
                        break;
                    case RobotMode.Ranged:
                        RangedAttack(direction);
                        attackTimer = rangedAttackTime;
                        break;
                    default:
                        break;
                }
            }
        }
    }

    protected override void FixedUpdate()
    {
        base.OnUpdate();

        if (target != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            switch (currentMode)
            {
                case RobotMode.Melee:
                    if (distanceToTarget > meleeRange)
                    {
                        // 追击敌人
                        Vector3 direction = (target.position - transform.position).normalized;
                        transform.position = Vector3.MoveTowards(transform.position, target.position, Time.fixedDeltaTime * 3.0f); // 3.0f 是移动速度，可以根据需要调整
                    }
                    break;

                case RobotMode.Ranged:
                    // 远程模式不移动
                    break;

                default:
                    break;
            }
        }
    }

    Transform FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < closestDistance)
            {
                closestDistance = distanceToEnemy;
                closestEnemy = enemy.transform;
            }
        }

        return closestEnemy;
    }

    void MeleeAttack(Vector3 direction)
    {
        attackInitPos.position = transform.position + direction * meleeRange;
        AttackHelper.InitTurretDamageCollider(meleeAttack, attackInitPos, 0.0f, direction, 0.0f, true, Skill.RenderMode.HorizontalFlip, player, 0.0f, this);
    }

    void RangedAttack(Vector3 direction)
    {
        attackInitPos.position = transform.position + direction * shootRange;
        AttackHelper.InitTurretDamageCollider(rangedAttack, attackInitPos, 0.0f, direction, rangedScatterAngle, true, Skill.RenderMode.NoneFlip, player, rangedAttackBulletSpeed, this);
    }
}

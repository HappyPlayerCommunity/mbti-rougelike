using UnityEngine;

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

    [SerializeField, Tooltip("敌人的特殊待机实现。")]
    private EnemySpecialImpl idelImpl;

    [SerializeField, Tooltip("敌人的特殊攻击实现。")]
    private EnemySpecialImpl atkImpl;

    [SerializeField, Tooltip("攻击前的抬手时间。")]
    private float preparationTime = 1.0f;

    [SerializeField, Tooltip("是否正在准备攻击？")]
    private bool isPreparingAttack = false;

    [SerializeField, Tooltip("准备攻击计时器。")]
    private float preparationTimer = 0.0f;

    [Header("互动组件")]
    public EnemyAttackChargingBar attackChargingBarPrefab;
    public EnemyAttackChargingBar attackChargingBar;


    public string PoolKey
    {
        get { return poolKey; }
        set { poolKey = value; }
    }

    public Player Player
    {
        get { return player; }
        set { player = value; }
    }

    public float PreparationTime
    {
        get { return preparationTime; }
        set { preparationTime = value; }
    }

    public bool IsPreparingAttack
    {
        get { return isPreparingAttack; }
        set { isPreparingAttack = value; }
    }

    public float PreparationTimer
    {
        get { return preparationTimer; }
        set { preparationTimer = value; }
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
        CreatePreparedAttackBar();
    }

    protected override void OnUpdate()
    {
        if (player == null) // 临时
            return;

        var distanceVec = player.transform.position - transform.position;

        Vector3 direction = Vector3.Normalize(distanceVec);
        float distance = Vector3.Distance(player.transform.position, transform.position);

        attackTimer -= Time.deltaTime;

        if (IsStaggered()) // 后续添加霸体逻辑跳过。
        {
            preparationTimer = preparationTime;
            return;
        }

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
                Flee();
                break;
            default:
                break;
        }
    }

    public void Idle()
    {
        ResetAttack();

        if (idelImpl)
        {
            idelImpl.ExecuteSpecialImplementation(this);
        }
    }

    public void Chase()
    {
        ResetAttack();

        var distanceVec = player.transform.position - transform.position;
        Vector3 direction = Vector3.Normalize(distanceVec);
        float distance = Vector3.Distance(player.transform.position, transform.position);

        if (statusManager.IsRooted())
        {
            velocity = Vector3.zero;
        }
        else
        {
            velocity = movementSpeed * direction;
        }
        VelocityUpdate();
    }

    public void Flee()
    {
        ResetAttack();

        var distanceVec = transform.position - player.transform.position;
        Vector3 direction = Vector3.Normalize(distanceVec);

        if (statusManager.IsRooted())
        {
            velocity = Vector3.zero;
        }
        else
        {
            velocity = movementSpeed * direction;
        }
        VelocityUpdate();
    }

    public void Attack()
    {
        if (statusManager.IsSlienced())
        {
            return;
        }

        var distanceVec = player.transform.position - transform.position;
        Vector3 direction = Vector3.Normalize(distanceVec);
        float distance = Vector3.Distance(player.transform.position, transform.position);

        // 如果处于攻击准备阶段
        if (isPreparingAttack)
        {
            preparationTimer -= Time.deltaTime;
            if (preparationTimer <= 0.0f)
            {
                // 攻击准备完成，进入攻击阶段
                isPreparingAttack = false;
                PerformAttack(direction, distanceVec);
            }
        }
        else if (distance <= attackRange && attackTimer <= 0.0f)
        {
            // 进入攻击准备阶段
            isPreparingAttack = true;
            preparationTimer = preparationTime;
        }
    }

    private void PerformAttack(Vector3 direction, Vector3 distanceVec)
    {
        if (atkImpl)
        {
            atkImpl.ExecuteSpecialImplementation(this);
        }
        else
        {
            // 实际攻击逻辑
            velocity = Vector3.zero;

            string poolKey = damageCollider.name;
            GameObject damageColliderObj = PoolManager.Instance.GetObject(poolKey, damageCollider.gameObject);
            DamageCollider collider = damageColliderObj.GetComponent<DamageCollider>();
            collider.Activate(transform.position + (distanceVec.normalized * damageColliderInitDistance), Quaternion.Euler(0.0f, 0.0f, 0.0f));
            collider.owner = transform.GetComponent<Unit>();
            collider.Velocity = (distanceVec.normalized * damageColliderMovementSpeed);

            var sprite = collider.GetComponentInChildren<SpriteRenderer>();
            if (sprite != null)
            {
                float angle = Vector2.SignedAngle(new Vector2(1.0f, 0.0f), direction);

                switch (damageColliderRenderMode)
                {
                    case Skill.RenderMode.HorizontalFlip:
                        sprite.transform.localEulerAngles = new Vector3(0.0f, 0.0f, angle);

                        if (direction.x < 0.0f)
                        {
                            sprite.transform.localScale = new Vector3(sprite.transform.localScale.x, -sprite.transform.localScale.y, sprite.transform.localScale.z);
                        }
                        break;
                    case Skill.RenderMode.AllFlip:
                        sprite.transform.localEulerAngles = new Vector3(0.0f, 0.0f, angle);

                        if (direction.x < 0.0f)
                        {
                            sprite.transform.localScale = new Vector3(-sprite.transform.localScale.x, -sprite.transform.localScale.y, sprite.transform.localScale.z);
                        }
                        break;
                    case Skill.RenderMode.Lock:
                        sprite.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                        break;
                    default:
                        break;
                }

                var collider2d = damageCollider.GetComponentInChildren<Collider2D>();
                if (collider2d != null)
                {
                    collider2d.transform.localEulerAngles = sprite.transform.localEulerAngles;
                }
            }

            attackTimer = attackTime;
            preparationTimer = PreparationTime;
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
        attackChargingBar.Deactivate();
        base.Die();
    }

    public void ResetAttack()
    {
        attackTimer = 0.0f;
        preparationTimer = PreparationTime;
        isPreparingAttack = false;
    }

    protected void CreatePreparedAttackBar()
    {
        GameObject attackChargingBarObj = PoolManager.Instance.GetObject(attackChargingBarPrefab.name, attackChargingBarPrefab.gameObject);
        attackChargingBar = attackChargingBarObj.GetComponent<EnemyAttackChargingBar>();
        attackChargingBar.transform.SetParent(canvasTransform, false);
        attackChargingBar.enemy = this;
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position + attackChargingBar.offset);
        attackChargingBar.GetComponent<RectTransform>().position = screenPosition;

        attackChargingBar.Activate(Vector3.zero, Quaternion.identity);
    }

    public void ResetObjectState()
    {
        hp = maxHp;
        shield = maxShield;
        velocity = Vector3.zero;
        blowForceVelocity = Vector3.zero;

        spriteRenderer.color = Color.white;
        staggerTimer = 0.0f;
        staggerRecordTime = 0.0f;
    }

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
            CreatePreparedAttackBar();
        }

        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
        PoolManager.Instance.ReturnObject(poolKey, gameObject);
    }
}
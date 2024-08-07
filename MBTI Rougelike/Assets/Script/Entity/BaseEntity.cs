using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 所有实体的基类，包含单位，建筑，敌人，玩家等子类共有的一些参数和方法。
/// </summary>
public abstract class BaseEntity : MonoBehaviour, IEntity
{
    [Header("实体数据")]
    [SerializeField, Tooltip("该实体的当前生命。")]
    protected int hp;

    [SerializeField, Tooltip("该实体的生命上限。")]
    protected int maxHp;

    [SerializeField, Tooltip("该实体的生命再生。")]
    protected int hpRegen;

    [SerializeField, Tooltip("当前实体的【速度】向量值")]
    protected Vector3 velocity;

    [SerializeField, Tooltip("当前实体的【吹飞速度】向量值")]
    protected Vector3 blowForceVelocity;

    [SerializeField, Tooltip("当前实体的【移动速度】。")]
    protected float movementSpeed = 1.0f;

    [SerializeField, Tooltip("当速度低于此阈值时，将【吹飞速度】清零。")]
    protected float stopBlowThreadshold = 0.5f;

    [SerializeField, Tooltip("此数值决定了【吹飞速度】的下降率")]
    protected float blowSpeedReduceRate = 0.1f;

    [SerializeField, Tooltip("该实体的当前护盾。")]
    protected int shield;

    [SerializeField, Tooltip("该实体的护盾上限。")]
    protected int maxShield;

    [SerializeField, Tooltip("该实体的护盾再生时间。")]
    protected float shieldReset;

    [SerializeField, Tooltip("该实体的【实体攻击力】。")]
    protected float physicalAtkPower = 1.0f;

    [SerializeField, Tooltip("该实体的【抽象攻击力】。")]
    protected float abstractAtkPower = 1.0f;

    [SerializeField, Tooltip("该实体的【全局攻击力】。")]
    protected float globalAtkPower = 1.0f;

    [Header("互动组件")]
    public HPController hpControllerPrefab;
    public Transform canvasTransform;
    protected HPController hpController;

    private Dictionary<GameObject, float> damageTimers = new Dictionary<GameObject, float>();

    protected Coroutine hpRegenCoroutine;
    protected Coroutine shieldRestoreCoroutine;

    protected virtual void Awake()
    {
    }

    protected virtual void Start()
    {
        CreateHealthBar();

        hp = maxHp;
        shield = maxShield;

        StartHealthRegen();

        shieldRestoreCoroutine = StartCoroutine(ShieldRestoreRoutine());
    }

    void Update()
    {
        velocity = new Vector3(velocity.x, velocity.y, 0.0f);
        OnUpdate();

        List<GameObject> keys = new List<GameObject>(damageTimers.Keys);
        foreach (var key in keys)
        {
            damageTimers[key] -= Time.deltaTime;

            if (damageTimers[key] <= 0.0f)
            {
                DamageManager.ClearReferences(key);
                damageTimers.Remove(key);
            }
        }
    }

    protected virtual void OnUpdate()
    {}

    void FixedUpdate()
    {
        transform.Translate(velocity * Time.fixedDeltaTime);
        transform.Translate(blowForceVelocity * Time.fixedDeltaTime);

        blowForceVelocity = new Vector3
(blowSpeedReduceUpdate(blowForceVelocity.x), blowSpeedReduceUpdate(blowForceVelocity.y), 0.0f);

        if (Mathf.Abs(blowForceVelocity.x) < stopBlowThreadshold)
            blowForceVelocity = new Vector3(0.0f, blowForceVelocity.y, 0.0f);
        if (Mathf.Abs(blowForceVelocity.y) < stopBlowThreadshold)
            blowForceVelocity = new Vector3(blowForceVelocity.x, 0.0f, 0.0f);

    }

    public int HP
    {
        get
        {
            return hp;
        }
        set
        {
            hp = value;
        }
    }

    public int MaxHP
    {
        get
        {
            return maxHp;
        }
        set
        {
            maxHp = value;
        }
    }

    public int Shield
    {
        get
        {
            return shield;
        }
        set
        {
            shield = value;
        }
    }

    public int MaxShield
    {
        get
        {
            return maxShield;
        }
        set
        {
            maxShield = value;
        }
    }

    public Vector3 Velocity
    {
        get
        {
            return velocity;
        }
        set
        {
            velocity = value;
        }
    }

    public Vector3 BlowForceVelocity
    {
        get
        {
            return blowForceVelocity;
        }
        set
        {
            blowForceVelocity = value;
        }
    }

    public float PhysicalAtkPower
    {
        get
        {
            return physicalAtkPower;
        }
        set
        {
            physicalAtkPower = value;
        }
    }

    public float AbstractAtkPower
    {
        get
        {
            return abstractAtkPower;
        }
        set
        {
            abstractAtkPower = value;
        }
    }

    public float GlobalAtkPower
    {
        get
        {
            return globalAtkPower;
        }
        set
        {
            globalAtkPower = value;
        }
    }

    public virtual void TakeDamage(int damage, float stuntime)
    {
        ResetShieldRestoreCoroutine();

        if (shield > 0)
        {
            shield -= damage;
            if (shield <= 0)
            {
                AnimationManager.Instance.PlayAnimation(Animation.ShieldBreak, transform, true);
                // 将穿透护盾的伤害施加到生命上。
                hp += shield;
            }
        }
        else
        {
            hp -= damage;
        }

        if (hp <= 0)
        {
            Die();
        }
        else if (hp > maxHp)
        {
            hp = maxHp;
        }
    }

    public virtual void Die()
    {
        hpController.Deactivate();
        gameObject.SetActive(false);
    }

    public float blowSpeedReduceUpdate(float speed)
    {
        if (speed > 0)
        {
            speed -= blowSpeedReduceRate;
            if (speed < 0) speed = 0;
        }
        else
        {
            speed += blowSpeedReduceRate;
            if (speed > 0) speed = 0;
        }

        return speed;
    }

    public virtual bool CanTakeDamageFrom(GameObject collider)
    {
        return !damageTimers.ContainsKey(collider);
    }

    public void SetDamageTimer(GameObject collider, float timer)
    {
        damageTimers[collider] = timer;
    }

    public void ClearDamageTimer(GameObject damageCollider)
    {
        if (damageTimers.ContainsKey(damageCollider))
        {
            damageTimers.Remove(damageCollider);
        }
    }

    public void Respawn()
    {
        this.HP = this.MaxHP;
        gameObject.SetActive(true);
        //OnRespawn?.Invoke();

        hpController.ResetObjectState();
    }

    protected void StartHealthRegen()
    {
        if (hpRegenCoroutine != null)
        {
            StopCoroutine(hpRegenCoroutine);
        }
        hpRegenCoroutine = StartCoroutine(HpRegenHealthOverTime());
    }

    private IEnumerator HpRegenHealthOverTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(1.0f);
            if (hp < maxHp)
            {
                hp += hpRegen;
                hp = Mathf.Min(hp, maxHp);
            }
        }
    }

    protected IEnumerator ShieldRestoreRoutine()
    {
        while (maxShield > 0 && shield < maxShield)
        {
            yield return new WaitForSeconds(Stats.basicShieldReset * shieldReset);

            AnimationManager.Instance.PlayAnimation(Animation.ShieldRestore, transform, true);
            shield = maxShield;
        }
    }

    private void ResetShieldRestoreCoroutine()
    {
        if (shieldRestoreCoroutine != null)
        {
            StopCoroutine(shieldRestoreCoroutine);
        }
        shieldRestoreCoroutine = StartCoroutine(ShieldRestoreRoutine());
    }

    protected void CreateHealthBar()
    {
        canvasTransform = GameObject.FindWithTag("MainCanvas").GetComponent<Canvas>().transform;

        GameObject healthBarObj = PoolManager.Instance.GetObject(hpControllerPrefab.name, hpControllerPrefab.gameObject);
        HPController healthBar = healthBarObj.GetComponent<HPController>();
        healthBar.transform.SetParent(canvasTransform, false);
        healthBar.baseEntity = this;
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position + healthBar.offset);
        healthBar.GetComponent<RectTransform>().position = screenPosition;

        healthBar.Activate(Vector3.zero, Quaternion.identity);

        hpController = healthBar;
    }
}
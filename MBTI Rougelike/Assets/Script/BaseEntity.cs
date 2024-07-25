using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 所有实体的基类，包含单位，建筑，敌人，玩家等子类共有的一些参数和方法。
/// </summary>
public abstract class BaseEntity : MonoBehaviour, IEntity
{
    [Header("实体数据")]
    [SerializeField, Tooltip("该实体的当前生命")]
    protected int hp;

    [SerializeField, Tooltip("该实体的生命上限")]
    protected int maxHp;

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

    [Header("互动组件")]
    public HPController hpControllerPrefab;
    public Transform canvasTransform;

    private Dictionary<GameObject, float> damageTimers = new Dictionary<GameObject, float>();

    protected virtual void Start()
    {
        canvasTransform = GameObject.Find("Canvas").transform;
        var healthBarInstance = Instantiate(hpControllerPrefab, canvasTransform);
        healthBarInstance.baseEntity = this;
    }

    void Update()
    {
        velocity = new Vector3(velocity.x, velocity.y, 0.0f);
        OnUpdate();

        List<GameObject> keys = new List<GameObject>(damageTimers.Keys);
        foreach (var key in keys)
        {
            damageTimers[key] -= Time.deltaTime;
            if (damageTimers[key] <= 0)
            {
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

    public virtual void TakeDamage(int damage, float stuntime)
    {
        hp -= damage;

        if (hp <= 0)
        {
            Die();
        }
        else if (hp > maxHp)
        {
            hp = maxHp;
        }
    }

    protected virtual void Die()
    {
        Destroy(gameObject); // for now
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


    public bool CanTakeDamageFrom(GameObject collider)
    {
        return !damageTimers.ContainsKey(collider);
    }

    public void SetDamageTimer(GameObject collider, float timer)
    {
        damageTimers[collider] = timer;
    }

}
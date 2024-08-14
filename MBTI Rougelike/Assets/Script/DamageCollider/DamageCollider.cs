using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.GraphicsBuffer;

public enum DamageType
{
    Physical,
    Abstract
}

public enum DamageElementType
{
    None,
    Fire,
    Ice,
    Earth,
    Wind,
    Thunder,
    Water
}

public enum DamageHealingType
{
    Damage,             //击中目标造成伤害
    Healing,            //击中目标造成治疗
    DamageAndHealing    //击中目标，根据其标签，造成伤害或治疗。
}

/// <summary>
/// 【伤害块】的基类，包含了绝大多数常见的伤害块行为；一些行为复杂的【伤害块】可以另开子类实现。
/// </summary>
public class DamageCollider : MonoBehaviour, IPoolable
{
    [Header("基础数据")]
    [SerializeField, Tooltip("该伤害块击中目标时可以造成的伤害。")]
    public int damage;

    [SerializeField, Tooltip("该伤害块的最大持续时间。")]
    protected float maxTimer;

    [SerializeField, Tooltip("会产生【碰撞判定】的其他物体tag。")]
    protected List<string> collideTags;

    [SerializeField, Tooltip("会产生【伤害判定】/【治疗判定】的其他物体tag。")]
    protected List<string> effectTags;

    [SerializeField, Tooltip("会被此物体【反弹】的其他物体的tag。")]
    protected List<string> reflectTags;

    [SerializeField, Tooltip("该伤害块是否依附于某个位置，而不是自己移动。")]
    protected bool isAttachedPos = false;

    [SerializeField, Tooltip("击中目标时造成的僵直时间。")]
    protected float staggerTime = 1.0f;

    [SerializeField, Tooltip("决定此【伤害块】是否与某个状态绑定，若绑定，则此伤害块的持续时间无限，直至绑定状态消失。")]
    protected bool isLifeTimeBindingWithState = false;

    [SerializeField, Tooltip("伤害块施加给被击中目标的【吹飞力度】。")]
    protected float blowForceSpeed;

    //[SerializeField, Tooltip("该伤害块若是动画类型，则在这里添加。")]
    //protected AnimationController2D animationPrefab;

    [SerializeField, Tooltip("该伤害块击中敌人时播放的动画，在这里添加。")]
    protected AnimationController2D hitEffectPrefab;

    [SerializeField, Tooltip("该伤害块击中敌人时播放的动画，在这里添加。")]
    protected AnimationController2D healEffectPrefab;

    [SerializeField, Tooltip("决定了击中特效的播放类型。")]
    protected HitEffectPlayMode hitEffectPlayMode;

    [SerializeField, Tooltip("决定了伤害为【实体伤害】，或【抽象伤害】。")]
    protected DamageType damageType;

    [SerializeField, Tooltip("此伤害块造成的伤害元素属性。")]
    protected DamageElementType damageElementType = DamageElementType.None;

    public Transform canvasTransform;

    protected HashSet<Collider2D> collidingObjects = new HashSet<Collider2D>();

    [SerializeField, Tooltip("该伤害通过蓄力可增加的最大伤害值")]
    protected int chargingDamage = 0;

    [SerializeField, Tooltip("该伤害通过蓄力可增加的击飞力。")]
    protected float chargingBlowForceSpeed = 0.0f;

    [SerializeField, Tooltip("该伤害通过蓄力可增加的最大体积")]
    protected Vector3 chargingLocalScale = Vector3.one;

    [SerializeField, Tooltip("该伤害通过蓄力可增加的最大硬直施加时间")]
    protected float chargingStaggerTime = 0.0f;

    [SerializeField, Tooltip("该伤害通过蓄力可增加的最大持续时间")]
    protected float chargingMaxTimer = 0.0f;

    [SerializeField, Tooltip("该伤害通过蓄力可增加弹道飞行速度。")]
    protected float chargingSpeedBoost = 0.0f;

    [SerializeField, Tooltip("该伤害通过蓄力可增加的穿透数量。")]
    protected int chargingPenetrability = 0;

    [SerializeField, Tooltip("是否可以击飞炮塔。")]
    protected bool blowTurret = false;

    [SerializeField, Tooltip("伤害块施加给炮塔的【吹飞力度】。")]
    protected float blowTurretForceSpeed;

    [SerializeField, Tooltip("此【伤害块】不造成伤害，而是治疗。")]
    protected bool isHealingMode = true;

    [SerializeField, Tooltip("此【伤害块】可以贯穿次数的次数。")]
    protected int penetrability = 1;

    [SerializeField, Tooltip("该【伤害块】可以生成的【地表】。")]
    protected Surface surface;

    [SerializeField, Tooltip("该【伤害块】可以随机生成数个【地表】。")]
    protected List<Surface> randomSurfaces;

    [SerializeField, Tooltip("击中目标时造成的僵直时间。")]
    protected List<DamageColliderBoost> boosts;

    public enum HitEffectPlayMode
    {
        HitPoint,  //在【伤害块】与【击中目标】的中间播放
        Target     //在目标身上播放。
    }

    public enum DamageHitType
    {
        SingleHit,           // 最常见的一次性伤害块，例如机枪射出的子弹，击中第一个目标便消失。
        MultiHit,            // 多目标一次性伤害块，例如较大的冲击波，可以击中多个目标，击中目标后便消失。
        SustainedMultiHit    // 类似于范围持续效应，如持续造成伤害的旋风，直线的激光，击中目标后也不会消失，而是在持续时间内反复造成伤害。
                             // 近战攻击的挥砍，和刺击也可以归于此类；它们的持续时间很短，只会造成一次伤害，但不会在击中目标的瞬间消失。
    }

    [SerializeField, Tooltip("伤害块的【伤害类型】，决定了它是如何结算伤害的。")]
    protected DamageHitType damageHitType;

    [SerializeField, Tooltip("若伤害块是【持续性的】，那它在【多久的间隔】可以对一个目标【再次造成伤害】；" +
        "设置成一个较大的值（例如超过其持续时间）可以用于那些只造成一次伤害，但不会立刻消失的伤害块类型。")]
    public float damageTriggerTime = float.MaxValue;


    [Header("Debug实时数据")]

    [SerializeField, Tooltip("伤害块的剩余持续时间。")]
    protected float timer;

    [SerializeField, Tooltip("伤害块的【速度矢量】。")]
    protected Vector2 velocity;

    [SerializeField, Tooltip("伤害块的初始生成位置。")]
    protected Vector3 initPos = new Vector3();

    [SerializeField, Tooltip("伤害快与原点的初始位置关系。")]
    protected Vector3 initInterval = new Vector3();

    [SerializeField, Tooltip("此【伤害块】现在是否可以造成碰撞。某些子弹如抛物线，可以在抛物过程中关闭此功能，仅在落地时结算碰撞。")]
    protected bool colliderActive = true;

    [SerializeField, Tooltip("伤害块的拥有者，或者说发射者，制造者。")]
    public BaseEntity owner;

    [SerializeField, Tooltip("伤害块是否触发过【击中特殊事件】，用于结算一些特殊的子类伤害块。")]
    public bool onceHitEventTrigger = false;

    [SerializeField, Tooltip("当前正在碰撞的物体。")]
    protected Collider2D[] hits;

    [SerializeField, Tooltip("是否已经击中过其他物体。")]
    public bool hitted = false;

    [SerializeField, Tooltip("是否已经造成过伤害。")]
    protected bool didDamage = false;

    [SerializeField, Tooltip("是否已经运行过Awake()。")]
    protected bool awaked = false;

    private Vector3 initSpriteLocalScale;
    private float initBlowForceSpeed;
    protected float initMaxTimer;
    protected int initDamage = 0;
    protected int initStaggerTime = 0;
    protected int initPenetrability = 0;

    protected List<string> initCollideTags;

    protected List<string> initEffectTags;

    protected List<string> initReflectTags;


    private string poolKey;

    const float basicShieldResistance = 0.5f;

    [Header("互动组件")]
    public Collider2D damageCollider2D;
    public AudioSource initSource;
    public DamageMovementType damageMovementType;
    public SpriteRenderer spriteRenderer;
    public Status ownerStatus;
    public Status applyStatus;
    public GameObject damagePopupPrefab;

    public enum DamageMovementType
    {
        Passive,
        Projectile,
    }

    // == Get/Set =============================================================
    public List<string> CollideTags
    {
        get { return collideTags; }
        set { collideTags = value; }
    }

    public List<string> DamageTags
    {
        get { return effectTags; }
        set { effectTags = value; }
    }

    public float Timer
    {
        get { return timer; }
        set { timer = value; }
    }

    public float MaxTimer
    {
        get { return maxTimer; }
        set
        {
            timer = value;
            maxTimer = value;
        }
    }

    public Vector2 Velocity
    {
        get { return velocity; }
        set { velocity = value; }
    }

    public bool IsAttachedPos
    {
        get { return isAttachedPos; }
        set { isAttachedPos = value; }
    }

    public Vector3 InitPos
    {
        get { return initPos; }
        set { initPos = value; }
    }

    public Vector3 InitInterval
    {
        get { return initInterval; }
        set { initInterval = value; }
    }

    public BaseEntity Owner
    {
        get { return owner; }
        set { owner = value; }
    }

    public string PoolKey
    {
        get { return poolKey; }
        set { poolKey = value; }
    }

    public float BlowForceSpeed
    {
        get { return blowForceSpeed; }
        set { blowForceSpeed = value; }
    }

    public float StaggerTime
    {
        get { return staggerTime; }
        set { staggerTime = value; }
    }

    public int ChargingDamage
    {
        get { return chargingDamage; }
        set { chargingDamage = value; }
    }

    public float ChargingBlowForceSpeed
    {
        get { return chargingBlowForceSpeed; }
        set { chargingBlowForceSpeed = value; }
    }
    public Vector3 ChargingLocalScale
    {
        get { return chargingLocalScale; }
        set { chargingLocalScale = value; }
    }

    public float ChargingStaggerTime
    {
        get { return chargingStaggerTime; }
        set { chargingStaggerTime = value; }
    }
    public float ChargingSpeedBoost
    {
        get { return chargingSpeedBoost; }
        set { chargingSpeedBoost = value; }
    }

    public int ChargingPenetrability
    {
        get { return chargingPenetrability; }
        set { chargingPenetrability = value; }
    }

    public float ChargingMaxTimer
    {
        get { return chargingMaxTimer; }
        set { chargingMaxTimer = value; }
    }
    public int Penetrability
    {
        get { return penetrability; }
        set { penetrability = value; }
    }


    private void Awake()
    {
        poolKey = gameObject.name;

        damageCollider2D = GetComponent<Collider2D>();

        if (damageCollider2D == null)
        {
            damageCollider2D = GetComponentInChildren<Collider2D>();
        }

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        initSpriteLocalScale = spriteRenderer.transform.localScale;
        initBlowForceSpeed = blowForceSpeed;
        initMaxTimer = maxTimer;
        initDamage = damage;
        initStaggerTime = Mathf.RoundToInt(staggerTime);
        initPenetrability = penetrability;

        initCollideTags = new List<string>(collideTags);
        initEffectTags = new List<string>(effectTags);
        initReflectTags = new List<string>(reflectTags);

        damageCollider2D.isTrigger = true;
        awaked = true;
        canvasTransform = GameObject.FindWithTag("MainCanvas").GetComponent<Canvas>().transform;
    }

    protected virtual void Start()
    {
        OnStart();
    }

    /// <summary>
    /// 继承自IPoolable接口的方法。用于对象池物体的初始化。
    /// </summary>
    public virtual void ResetObjectState()
    {
        var animController = transform.GetComponentInChildren<AnimationController2D>(true);
        var spriteRenderer = transform.GetComponentInChildren<SpriteRenderer>(true);

        if (animController)
        {
            animController.animationFinished = false;
        }

        if (spriteRenderer)
        {
            spriteRenderer.gameObject.SetActive(true);
            spriteRenderer.transform.localScale = initSpriteLocalScale;
        }

        blowForceSpeed = initBlowForceSpeed;
        maxTimer = initMaxTimer;
        damage = initDamage;
        staggerTime = initStaggerTime;
        penetrability = initPenetrability;

        collideTags = new List<string>(initCollideTags);
        effectTags = new List<string>(initEffectTags);
        reflectTags = new List<string>(initReflectTags);

        OnStart();
    }

    /// <summary>
    /// 当对象从对象池中取出时，调用这个方法来初始化。
    /// </summary>
    public virtual void Activate(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;

        var sprite = GetComponentInChildren<SpriteRenderer>();
        if (sprite)
        {
            sprite.transform.localScale = new Vector3(Mathf.Abs(sprite.transform.localScale.x), Mathf.Abs(sprite.transform.localScale.y), sprite.transform.localScale.z);
            sprite.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
        }

        for (int i = 0; i < boosts.Count; i++)
        {
            boosts[i].Boost(this);
        }
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 调用这个方法将对象塞回对象池。
    /// </summary>
    public virtual void Deactivate()
    {
        gameObject.SetActive(false);

        DamageManager.ClearReferences(gameObject);
        PoolManager.Instance.ReturnObject(poolKey, gameObject);
    }

    void Update()
    {
        OnUpdate();

        if (isLifeTimeBindingWithState)
        {
            if (ownerStatus && ownerStatus.IsExpired())
            {
                timer = 0.0f;
            }
        }
        else
        {
            timer -= Time.deltaTime;
        }

        if (timer <= 0.0f)
        {
            Deactivate();
            CreateSurface();
        }
        else
        {
            CollisionCheck();
        }
    }

    protected virtual void FixedUpdate()
    {
        if (!isAttachedPos)
        {
            transform.Translate(velocity * Time.fixedDeltaTime);
        }
        else
        {
            if (isAttachedPos && owner)
            {
                Vector3 fixInitPos = initPos;
                //fixInitPos.x = initPos.x * owner.FaceRightFloat;
                //transform.position = owner.transform.position + fixInitPos;
                transform.position = owner.transform.position + initInterval;
            }
        }
        OnFixedUpdate();
    }

    /// <summary>
    /// 这里将结算【伤害块】的碰撞。造成伤害，吹飞等效果也将在这里结算。
    /// </summary>
    protected virtual void CollisionCheck()
    {
        if (!damageCollider2D || !colliderActive)
            return;

        // 创建一个临时列表来保存当前的碰撞对象
        List<Collider2D> currentCollidingObjects = new List<Collider2D>(collidingObjects);

        didDamage = false;

        foreach (Collider2D hit in currentCollidingObjects)
        {
            if (hit == damageCollider2D)
                continue;

            // 检测【伤害块】是否可以与【碰撞体】互动————PS：这不意味着造成伤害，可能击中墙壁，敌人的子弹等。
            if (TagCollidingCheck(hit))
            {
                EffectToObject(hit);
            }
        }

        // 如果击中目标且不是【持续群体打击】类型，则销毁子弹
        if (hitted && damageHitType != DamageHitType.SustainedMultiHit && penetrability <= 0)
        {
            Deactivate();
            CreateSurface();
            return;
        }
    }

    public virtual void PlayDestroyedAduioSource()
    {
        var audioSource = transform.GetComponentInChildren<AudioSource>();
        if (audioSource)
        {
            audioSource.transform.parent = null;
            Destroy(audioSource.gameObject, audioSource.clip.length);
            audioSource.Play();
        }
    }

    protected virtual void BlowUpEntity(BaseEntity entity, bool ignore = false)
    {
        // 护盾会令吹飞效果减半。
        float blowupResistance = entity.Shield > 0.0f ? basicShieldResistance : 1.0f;

        // 韧性效果会令吹飞效果等比率下降。
        float toughness = entity.Toughness;

        float finalBlowForceSpeed = blowForceSpeed;
        if (ignore)
        {
            blowupResistance = 1.0f;
            toughness = 1.0f;

            finalBlowForceSpeed = blowTurretForceSpeed;
        }

        switch (damageMovementType)
        {
            case DamageMovementType.Passive:
                Vector3 direction;

                if (owner)
                    direction = (entity.transform.position - owner.transform.position).normalized;
                else
                    direction = (entity.transform.position - transform.position).normalized;

                entity.BlowForceVelocity = finalBlowForceSpeed * direction * blowupResistance * toughness;
                break;

            case DamageMovementType.Projectile:
                var characterPos = entity.transform.position;

                var direction1 = (characterPos - transform.position).normalized;
                var direction2 = (velocity).normalized;

                entity.BlowForceVelocity = finalBlowForceSpeed * (direction1 + (Vector3)direction2) * blowupResistance * toughness;
                break;

            default:
                break;
        }
    }

    protected bool TagCollidingCheck(Collider2D hit)
    {
        foreach (var tag in collideTags)
            if (TagHelper.CompareTag(hit, tag))
                return true;

        return false;
    }

    protected bool TagEffectCheck(Collider2D hit)
    {
        foreach (var tag in effectTags)
            if (TagHelper.CompareTag(hit, tag))
                return true;

        return false;
    }

    protected bool TagReflectCheck(Collider2D hit)
    {
        foreach (var tag in reflectTags)
            if (TagHelper.CompareTag(hit, tag))
                return true;

        return false;
    }

    protected virtual void NormalCollideEvents(Collider2D hit)
    {

    }

    protected virtual void CharacterCollideEvents(Collider2D hit)
    {

    }

    protected virtual void CollideEvents(Collider2D hit)
    {

    }

    protected virtual void OnceCollideEvents(Collider2D hit)
    {

    }

    protected virtual void HitAnimation(Transform transform)
    {
        if (isHealingMode)
        {
            if (healEffectPrefab)
            {
                GameObject hitEffect = PoolManager.Instance.GetObject(healEffectPrefab.name, healEffectPrefab.gameObject);
                AnimationController2D anim = hitEffect.GetComponent<AnimationController2D>();
                anim.Activate(transform.position, Quaternion.identity);
                anim.attachedTransform = transform;
            }
        }
        else
        {
            if (hitEffectPrefab)
            {
                GameObject hitEffect = PoolManager.Instance.GetObject(hitEffectPrefab.name, hitEffectPrefab.gameObject);
                AnimationController2D anim = hitEffect.GetComponent<AnimationController2D>();
                anim.Activate(transform.position, Quaternion.identity);
            }
        }

        //Instantiate(hitEffectPrefab, position, Quaternion.identity);
    }

    protected virtual void OnUpdate()
    { }

    protected virtual void OnFixedUpdate()
    { }

    private void OnDrawGizmos()
    {
        if (awaked)
        {
            //Gizmos.color = Color.red;
            //Gizmos.DrawWireCube(transform.position, boxCollider2D.size);
        }
    }

    protected virtual void BeforeStart()
    {

    }

    //private void OnDestroy()
    //{
    //    if (gameObject.activeInHierarchy) // 检查对象是否仍在场景中活跃
    //    {
    //        Debug.LogError("Object destroyed improperly: " + gameObject.name);
    //    }
    //}

    protected virtual void OnStart()
    {
        BeforeStart();

        if (initSource)
        {
            initSource.Play();
        }
        var audioSource = GetComponentInChildren<AudioSource>();
        if (audioSource)
        {
            audioSource.time = audioSource.clip.length * 0.25f;
        }

        onceHitEventTrigger = false;
        hitted = false;
        didDamage = false;
        Array.Clear(hits, 0, hits.Length);

        if (owner is Player)
        {
            var player = (Player)owner;
            float attackRangeBouns = player.stats.Calculate_AttackRange();

            // 【攻击范围】对两类伤害块的有不同的加成。
            switch (damageMovementType)
            {
                case DamageMovementType.Passive:
                    // 静态：每1点增加1%的碰撞体积；
                    spriteRenderer.transform.localScale *= attackRangeBouns;
                    break;
                case DamageMovementType.Projectile:
                    // 动态：每1点增加1%的持续时间，变相增加了射程。
                    maxTimer *= attackRangeBouns;
                    break;
                default:
                    break;
            }

            blowForceSpeed *= player.stats.Calculate_Knockback();
        }

        timer = maxTimer;
    }

    protected bool TryHit(Unit unit)
    {
        if (unit == null)
        {
            return false;
        }

        float randomValue = UnityEngine.Random.value;

        // 如果随机数大于闪避概率，则成功命中
        if (randomValue > unit.DodgeRate)
        {
            return true;
        }

        return false;
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other != damageCollider2D)
        {
            collidingObjects.Add(other);
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        if (collidingObjects.Contains(other))
        {
            collidingObjects.Remove(other);
        }
    }

    protected void EffectToObject(Collider2D hit)
    {
        //除了反射逻辑，后续还可以添加抵消逻辑。
        if (TagReflectCheck(hit))
        {
            Debug.Log("Reflect");
            DamageCollider enemyDamageCollider;

            enemyDamageCollider = hit.gameObject.GetComponent<DamageCollider>();
            if (!enemyDamageCollider)
            {
                enemyDamageCollider = hit.gameObject.GetComponentInParent<DamageCollider>();
            }

            if (enemyDamageCollider)
            {
                if (enemyDamageCollider.owner == owner)
                {
                    return;
                }

                enemyDamageCollider.velocity = -enemyDamageCollider.velocity;
                enemyDamageCollider.owner = owner;
                enemyDamageCollider.timer = enemyDamageCollider.maxTimer;

                enemyDamageCollider.effectTags.Clear();
                enemyDamageCollider.collideTags.Clear();

                enemyDamageCollider.effectTags.Add(Tag.Enemy);
                enemyDamageCollider.collideTags.Add(Tag.Enemy);
            }

            return;
        }

        // 标记已经击中目标
        hitted = true;

        //检测【伤害块】是否可以对【碰撞体】造成伤害/治疗。
        if (TagEffectCheck(hit))
        {
            var entity = hit.gameObject.GetComponent<BaseEntity>();
            if (!entity)
            {
                entity = hit.gameObject.GetComponentInParent<BaseEntity>();
            }

            //确定该实体现在是否能受到伤害/治疗。
            if (entity && entity.CanTakeDamageFrom(gameObject))
            {
                //执行伤害/治疗逻辑。
                DamageToObject(entity, hit);
            }
        }

        // 执行击中目标

        // 如果一次性碰撞事件尚未触发，则触发
        // 可以插入一些特殊结算，比如击中目标后产生爆炸，分裂子弹，反弹等。
        if (!onceHitEventTrigger)
        {
            OnceCollideEvents(hit);
            onceHitEventTrigger = true;
        }

        CollideEvents(hit);

        if (blowTurret && hit.tag == "Turret")
        {
            BlowUpEntity(hit.GetComponent<Turret>(), true);
        }

        // 根据伤害类型决定是否销毁子弹（后续可以优化进子弹池）
        switch (damageHitType)
        {
            case DamageHitType.SingleHit:
                // 单次击中碰撞体后消失
                if (didDamage)
                {
                    penetrability -= 1;
                    if (penetrability <= 0)
                    {
                        Deactivate();
                        CreateSurface();
                        return;
                    }
                    else
                    {
                        break;
                    }
                }
                break;
            case DamageHitType.MultiHit:
                // 击中多个目标但不会持续伤害，不销毁
                break;
            case DamageHitType.SustainedMultiHit:
                // 持续对接触的目标造成伤害，不销毁
                break;
        }
    }

    protected virtual void DamageToObject(BaseEntity entity, Collider2D hit)
    {
        //注册此伤害行为。
        DamageManager.RegisterDamage(gameObject, entity);

        if (isHealingMode)
        {
            DamagePopupManager.Instance.Popup(PopupType.Healing, hit.transform.position, damage, false);
            entity.GetHealing(damage);
            entity.SetDamageTimer(gameObject, damageTriggerTime);
            HitAnimation(hit.transform);
        }
        else
        {
            if (entity is Unit)
            {
                var unit = (Unit)entity;

                // 如果【碰撞体】是Unit类型，则结算【闪避】和【吹飞】效果。
                if (TryHit(unit))
                {
                    BlowUpEntity(unit);
                }
                else
                {
                    // 使【伤害块】短时间无法再对该目标造成伤害。
                    entity.SetDamageTimer(gameObject, damageTriggerTime);
                    DamagePopupManager.Instance.Popup(PopupType.Miss, hit.transform.position);
                    //continue;
                    return;
                }
            }

            // 后面需要考虑到子弹单位的拥有者位死亡后，子弹还在的情况。
            // 以目前对象池的逻辑，拥有者不在了死亡以后，它们并没有被销毁，而是在对象池内，依然可以访问owner。

            // 暴击默认为false，如果暴击，则会在CalculateDamage中修改。
            bool isCrit = false;
            int finalDamage = DamageManager.CalculateDamage(damageType, damage, owner, ref isCrit, damageElementType);

            // 对实体造成伤害并设置击晕时间
            entity.TakeDamage(finalDamage, staggerTime);

            DamagePopupManager.Instance.Popup(PopupType.Damage, hit.transform.position, finalDamage, isCrit);

            // 令该实体保存一个对此【伤害块】的计时器，短时间无法再对其造成伤害。
            entity.SetDamageTimer(gameObject, damageTriggerTime);

            if (owner is Player)
            {
                var player = (Player)owner;
                float boostCharge = player.stats.Calculate_AttackEnergeCharge();
                player.personality.AttackChargeEnerge(finalDamage, boostCharge); // 受伤充能比率还得具体设计。
            }

            switch (hitEffectPlayMode)
            {
                case HitEffectPlayMode.HitPoint: // hmm，不太完善。
                    RaycastHit2D raycastHit = Physics2D.Raycast(transform.position, (hit.transform.position - transform.position).normalized);
                    if (raycastHit.collider != null)
                    {
                        Transform collisionPoint = transform;
                        collisionPoint.position = raycastHit.point;
                        HitAnimation(collisionPoint);
                    }
                    break;
                case HitEffectPlayMode.Target:
                    HitAnimation(hit.transform);
                    break;
                default:
                    break;
            }
        }
        didDamage = true;

        if (applyStatus && entity.StatusManager && entity.IsAlive())
        {
            if (owner is Player)
            {
                var player = (Player)owner;
                entity.StatusManager.AddStatus(applyStatus, player.stats);
            }
            else
            {
                entity.StatusManager.AddStatus(applyStatus, null);
            }

            //Debug.Log(entity.StatusManager.ActiveStatus());

        }
    }

    protected void CreateSurface()
    {
        if (randomSurfaces != null && randomSurfaces.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, randomSurfaces.Count);
            Surface selectedSurface = randomSurfaces[randomIndex];

            GameObject newSurfaceObj = PoolManager.Instance.GetObject(selectedSurface.name, selectedSurface.gameObject);
            Surface newSurface = newSurfaceObj.GetComponent<Surface>();
            newSurface.Activate(transform.position, Quaternion.identity);
        }
        else if (surface)
        {
            GameObject newSurfaceObj = PoolManager.Instance.GetObject(surface.name, surface.gameObject);
            Surface newSurface = newSurfaceObj.GetComponent<Surface>();
            newSurface.Activate(transform.position, Quaternion.identity);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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

    [SerializeField, Tooltip("会产生【伤害判定】的其他物体tag。")]
    protected List<string> damageTags;

    [SerializeField, Tooltip("该伤害块是否依附于某个位置，而不是自己移动。")]
    protected bool isAttachedPos = false;

    [SerializeField, Tooltip("击中目标时造成的僵直时间。")]
    protected float stunTime = 0.0f;

    [SerializeField, Tooltip("决定此【伤害块】是否与某个状态绑定，若绑定，则此伤害块的持续时间无限，直至绑定状态消失。")]
    protected bool isLifeTimeBindingWithState = false;

    [SerializeField, Tooltip("伤害块施加给被击中目标的【吹飞力度】。")]
    protected float blowForceSpeed;

    //[SerializeField, Tooltip("该伤害块若是动画类型，则在这里添加。")]
    //protected AnimationController2D animationPrefab;

    [SerializeField, Tooltip("该伤害块击中敌人时播放的动画，在这里添加。")]
    protected AnimationController2D hitEffectPrefab;

    [SerializeField, Tooltip("决定了击中特效的播放类型。")]
    HitEffectPlayMode hitEffectPlayMode;

    public Transform canvasTransform;


    public enum HitEffectPlayMode
    {
        HitPoint,  //在【伤害块】与【击中目标】的中间播放
        Target     //在目标身上播放。
    }

    public enum DamageType
    {
        SingleHit,           // 最常见的一次性伤害块，例如机枪射出的子弹，击中第一个目标便消失。
        MultiHit,            // 多目标一次性伤害块，例如较大的冲击波，可以击中多个目标，击中目标后便消失。
        SustainedMultiHit    // 类似于范围持续效应，如持续造成伤害的旋风，直线的激光，击中目标后也不会消失，而是在持续时间内反复造成伤害。
                             // 近战攻击的挥砍，和刺击也可以归于此类；它们的持续时间很短，只会造成一次伤害，但不会在击中目标的瞬间消失。
    }

    [SerializeField, Tooltip("伤害块的【伤害类型】，决定了它是如何结算伤害的。")]
    protected DamageType damageType;

    [SerializeField, Tooltip("若伤害块是【持续性的】，那它在【多久的间隔】可以对一个目标【再次造成伤害】；" +
        "设置成一个较大的值（例如超过其持续时间）可以用于那些只造成一次伤害，但不会立刻消失的伤害块类型。")]
    public float damageTriggerTime = float.MaxValue;


    [Header("Debug实时数据")]

    [SerializeField, Tooltip("伤害块的剩余持续时间。")]
    protected float timer;

    [SerializeField, Tooltip("伤害块的【速度矢量】。")]
    protected Vector2 velocity;

    [SerializeField, Tooltip("伤害快的初始生成位置。")]
    protected Vector3 initPos = new Vector3();

    [SerializeField, Tooltip("伤害块的拥有者，或者说发射者，制造者。")]
    public Unit owner;

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

    private string poolKey;

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
        get { return damageTags; }
        set { damageTags = value; }
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

    public Unit Owner
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
        damageCollider2D.isTrigger = true;
        awaked = true;
        canvasTransform = GameObject.FindWithTag("MainCanvas").GetComponent<Canvas>().transform;
    }

    void Start()
    {
        OnStart();
    }

    /// <summary>
    /// 当对象从对象池中取出时，调用这个方法来初始化。
    /// </summary>
    public void Activate(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;

        var sprite = GetComponentInChildren<SpriteRenderer>();
        if (sprite)
        {
            sprite.transform.localScale = new Vector3(Mathf.Abs(sprite.transform.localScale.x), Mathf.Abs(sprite.transform.localScale.y), sprite.transform.localScale.z);
            sprite.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
        }

        gameObject.SetActive(true);
    }

    /// <summary>
    /// 调用这个方法将对象塞回对象池。
    /// </summary>
    public void Deactivate()
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
        }
        else
        {
            CollisionCheck();
        }
    }

    void FixedUpdate()
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
                transform.position = owner.transform.position + fixInitPos;
            }
        }
        OnFixedUpdate();
    }

    /// <summary>
    /// 这里将结算【伤害块】的碰撞。造成伤害，吹飞等效果也将在这里结算。
    /// </summary>
    protected void CollisionCheck()
    {
        if (!damageCollider2D)
            return;

        Vector2 colliderSize;

        switch (damageCollider2D)
        {
            case BoxCollider2D collider2D:
                colliderSize = (collider2D.size * spriteRenderer.transform.localScale).Absolute();
                hits = Physics2D.OverlapBoxAll(transform.position, colliderSize, 0.0f);
                break;
            case CapsuleCollider2D collider2D:
                colliderSize = (collider2D.size * spriteRenderer.transform.localScale).Absolute();
                hits = Physics2D.OverlapCapsuleAll(transform.position, colliderSize, collider2D.direction, collider2D.transform.eulerAngles.z);
                break;
            default:
                break;
        }

        foreach (Collider2D hit in hits)
        {
            if (hit == damageCollider2D)
                continue;

            // 检测【伤害块】是否可以与【碰撞体】互动————PS：这不意味着造成伤害，可能击中墙壁，敌人的子弹等。
            if (TagCollidingCheck(hit))
            {
                // 标记已经击中目标
                hitted = true;

                //检测【伤害块】是否可以对【碰撞体】造成伤害
                if (TagDamageCheck(hit))
                {
                    var entity = hit.gameObject.GetComponent<BaseEntity>();

                    //确定该实体是否能受到伤害。
                    if (entity && entity.CanTakeDamageFrom(gameObject))
                    {
                        //注册此伤害行为。
                        DamageManager.RegisterDamage(gameObject, entity);

                        // 如果【碰撞体】是Unit类型，则结算【闪避】和【吹飞】效果。
                        if (entity is Unit)
                        {
                            var unit = (Unit)entity;

                            if (TryHit(unit))
                            {
                                BlowUnit(unit);
                            }
                            else
                            {
                                // 使【伤害块】短时间无法再对该目标造成伤害。
                                entity.SetDamageTimer(gameObject, damageTriggerTime);
                                continue;
                            }
                        }

                        // 对实体造成伤害并设置击晕时间
                        entity.TakeDamage(damage, stunTime);

                        DamagePopupManager.Instance.PopupDamageNumber(damage, hit.transform.position);
                        //ShowDamagePopup(damage, hit.transform.position);

                        // 令该实体保存一个对此【伤害块】的计时器，短时间无法再对其造成伤害。
                        entity.SetDamageTimer(gameObject, damageTriggerTime);

                        if (owner is Player)
                        {
                            var player = (Player)owner;
                            float boostCharge = player.stats.Calculate_AttackEnergeCharge();
                            player.personality.AttackChargeEnerge(damage, boostCharge); // 受伤充能比率还得具体设计。
                        }

                        switch (hitEffectPlayMode)
                        {
                            case HitEffectPlayMode.HitPoint: // hmm，不太完善。
                                RaycastHit2D raycastHit = Physics2D.Raycast(transform.position, (hit.transform.position - transform.position).normalized);
                                if (raycastHit.collider != null)
                                {
                                    Vector2 collisionPoint = raycastHit.point;
                                    HitAnimation(collisionPoint);
                                }
                                break;
                            case HitEffectPlayMode.Target:
                                HitAnimation(hit.transform.position);
                                break;
                            default:
                                break;
                        }
                    }

                    // 标记已经造成伤害
                    didDamage = true;
                }

                // 根据伤害类型决定是否销毁子弹（后续可以优化进子弹池）
                switch (damageType)
                {
                    case DamageType.SingleHit:
                        // 单次击中碰撞体后消失
                        //Destroy(gameObject);
                        Deactivate();
                        break;
                    case DamageType.MultiHit:
                        // 击中多个目标但不会持续伤害，不销毁
                        break;
                    case DamageType.SustainedMultiHit:
                        // 持续对接触的目标造成伤害，不销毁
                        break;
                }

                // 执行击中目标

                // 如果一次性碰撞事件尚未触发，则触发
                // 可以插入一些特殊结算，比如击中目标后产生爆炸，分裂子弹，反弹等。
                if (!onceHitEventTrigger)
                {
                    OnceCollideEvents(hit);
                    onceHitEventTrigger = true;
                }
            }
        }

        // 如果击中目标且不是【持续群体打击】类型，则销毁子弹
        if (hitted && damageType != DamageType.SustainedMultiHit)
        {
            //Destroy(gameObject);
            Deactivate();
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

    protected virtual void BlowUnit(Unit unit)
    {
        switch (damageMovementType)
        {
            case DamageMovementType.Passive:
                Vector3 direction;

                if (owner)
                    direction = (unit.transform.position - owner.transform.position).normalized;
                else
                    direction = (unit.transform.position - transform.position).normalized;

                unit.BlowForceVelocity = blowForceSpeed * direction;
                break;
        
            case DamageMovementType.Projectile:
                var characterPos = unit.transform.position;
        
                var direction1 = (characterPos - transform.position).normalized;
                var direction2 = (velocity).normalized;
        
                unit.BlowForceVelocity = blowForceSpeed * (direction1 + (Vector3)direction2);
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

    protected bool TagDamageCheck(Collider2D hit)
    {
        foreach (var tag in damageTags)
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

    protected virtual void HitAnimation(Vector3 position)
    {
        if (hitEffectPrefab)
        {
            GameObject hitEffect = PoolManager.Instance.GetObject(hitEffectPrefab.name, hitEffectPrefab.gameObject);
            AnimationController2D anim = hitEffect.GetComponent<AnimationController2D>();
            anim.Activate(position, Quaternion.identity);
            //DamageCollider damageCollider = damageColliderObj.GetComponent<DamageCollider>();
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

    /// <summary>
    /// 继承自IPoolable接口的方法。用于对象池物体的初始化。
    /// </summary>
    public void ResetObjectState()
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

        OnStart();
    }

    void OnStart()
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

    protected void ShowDamagePopup(int damage, Vector3 position)
    {
        if (damagePopupPrefab)
        {
            GameObject popupObj = PoolManager.Instance.GetObject(damagePopupPrefab.name, damagePopupPrefab.gameObject);
            DamagePopup damagePopup = popupObj.GetComponent<DamagePopup>();
            damagePopup.Activate(position, Quaternion.identity);

            //GameObject popup = Instantiate(damagePopupPrefab, position, Quaternion.identity);
            //DamagePopup damagePopup = popup.GetComponent<DamagePopup>();
            damagePopup.SetDamage(damage);
            damagePopup.transform.SetParent(canvasTransform, false);
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(position);
            damagePopup.GetComponent<RectTransform>().position = screenPosition;
        }
    }
}

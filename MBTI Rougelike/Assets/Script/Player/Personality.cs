using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 人格类。用来管理各个人格的普攻（Auto），特技（Sp），大招（Ult），被动等各种效果。
/// </summary>
public class Personality : MonoBehaviour
{
    [Header("普通攻击（普攻）")]
    [SerializeField, Tooltip("该技能的数据。")]
    private Skill normalAttack;

    [SerializeField, Tooltip("该技能的当前冷却计时器。不大于0才能使用。")]
    private float normalAttack_CurretReloadingTimer;

    [SerializeField, Tooltip("普攻的生成位置。")]
    private Transform normalAttack_InitPosition;


    [Header("特殊技能（特技）")]
    [SerializeField, Tooltip("该技能的数据。")]
    private Skill specialSkill;

    [SerializeField, Tooltip("该技能的当前冷却计时器。不大于0才能使用。")]
    private float specialSkill_CurretReloadingTimer;

    [SerializeField, Tooltip("特技的生成位置。")]
    private Transform specialSkill_InitPosition;


    [Header("终极技能（大招）")]
    [SerializeField, Tooltip("该大招能生成的伤害块")]
    private Skill ultimateSkill;

    [SerializeField, Tooltip("当前大招充能。充满到100才能释放大招，并消耗所有能量。")]
    private float ultimateEnerge = 0.0f;

    [SerializeField, Tooltip("大招的生成位置。")]
    private Transform ultSkill_InitPosition;

    [SerializeField, Tooltip("该大招能生成的伤害块")]
    private float maxUltimateEnerge = 100.0f;


    [Header("互动组件")]
    [Tooltip("人格八维数据。")]
    public Preference preference;

    [Tooltip("能力值数据。")]
    public Stats stats;

    [Tooltip("状态管理机。")]
    public StatusManager statusManager;

    protected Coroutine energeChargeCoroutine;

    public float UltimateEnerge
    {
        get { return ultimateEnerge; }
        set { ultimateEnerge = value; }
    }

    public float MaxUltimateEnerge
    {
        get { return maxUltimateEnerge; }
        set { maxUltimateEnerge = value; }
    }

    //大招

    //被动

    public Player player;
    public Aim aim;

    void Start()
    {
        aim = transform.GetComponentInChildren<Aim>();
        player = GetComponent<Player>();
        stats = player.stats;
        statusManager = GetComponent<StatusManager>();
        StartEnergeCharge();
    }

    void Update()
    {
        normalAttack_CurretReloadingTimer -= Time.deltaTime;
        specialSkill_CurretReloadingTimer -= Time.deltaTime;

        if (player.IsStaggered())
        {
            return;
        }

        player.IsActioning = false;

        if (!statusManager.IsSlienced())
        {
            SkillUpdate(normalAttack, ref normalAttack_CurretReloadingTimer, normalAttack_InitPosition, Input.GetMouseButton(0), true); //左键
            SkillUpdate(specialSkill, ref specialSkill_CurretReloadingTimer, specialSkill_InitPosition, Input.GetMouseButton(1), false); //右键

            UltimateSkillUpdate();
        }
    }

    protected virtual void SkillUpdate(Skill skill, ref float currentReloadingTimer, Transform initPos, bool holding, bool isAuto)
    {
        Vector3 mousePos = Input.mousePosition;
        var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0.0f;
        Vector3 aimDirection = aim.aimDirection;

        Transform playerTransform = player.transform;
        Transform playerArtTransform = player.playerArtTransform;
        Transform weaponArtTransform = player.weaponArtTransform;

        // 后续的天赋/buff修正，或许可以在这里结算。
        float scatterAngle = skill.ScatterAngle;
        float damageColliderSpeed = skill.DamageColliderSpeed;
        float reloadingTime = skill.ReloadingTime;

        if (holding) // 在后面改为绑定按键
        {
            player.IsActioning = true;
            if (currentReloadingTimer <= 0.0f)
            {
                if (skill.DamageCollider)
                {
                    string poolKey = skill.DamageCollider.name;
                    GameObject damageColliderObj = PoolManager.Instance.GetObject(poolKey, skill.DamageCollider.gameObject);
                    DamageCollider damageCollider = damageColliderObj.GetComponent<DamageCollider>();
                    damageCollider.Activate(initPos.position, Quaternion.Euler(0.0f, 0.0f, 0.0f));
                    damageCollider.owner = player;

                    var sprite = damageCollider.GetComponentInChildren<SpriteRenderer>();

                    if (sprite)
                    {
                        float angle = Vector2.SignedAngle(new Vector2(1.0f, 0.0f), aimDirection);
                        sprite.transform.localEulerAngles = new Vector3(0.0f, 0.0f, angle);

                        switch (skill.GetRenderMode)
                        {
                            case Skill.RenderMode.HorizontalFlip:
                                if (aimDirection.x < 0.0f)
                                {
                                    sprite.transform.localScale = new Vector3(sprite.transform.localScale.x, -sprite.transform.localScale.y, sprite.transform.localScale.z);
                                }
                                break;
                            case Skill.RenderMode.AllFlip:
                                if (aimDirection.x < 0.0f)
                                {
                                    sprite.transform.localScale = new Vector3(-sprite.transform.localScale.x, -sprite.transform.localScale.y, sprite.transform.localScale.z);
                                }
                                break;
                            default:
                                break;
                        }

                        // 以前的实现方法，备存一下。

                        //// 如果瞄准方向是向左的，则需要将角度加180度，因为Sprite【默认面向右侧】
                        //if (aimDirection.x < 0.0f)
                        //{
                        //    angle += 180.0f;
                        //}

                        //// 直接根据y轴方向调整旋转角度
                        //sprite.transform.localEulerAngles = aimDirection.y > 0.0f ? new Vector3(0.0f, 0.0f, angle) : new Vector3(0.0f, 0.0f, -angle);

                    }

                    float scatterAngleHalf = scatterAngle / 2.0f;

                    float randomAngle = Random.Range(-scatterAngleHalf, scatterAngleHalf);

                    Vector3 scatterDirection = Quaternion.Euler(0, 0, randomAngle) * aimDirection;

                    Vector3 finalVelocity = scatterDirection.normalized * damageColliderSpeed;
                    damageCollider.Velocity = finalVelocity;
                }

                player.BlowForceVelocity = aimDirection * skill.SelfBlowForce; //for now, 负数可以做向后退的技能。

                currentReloadingTimer = reloadingTime;

                if (isAuto)
                    currentReloadingTimer = reloadingTime * stats.Calculate_AttackSpeed();
                else
                    currentReloadingTimer = reloadingTime * stats.Calculate_SpecialCooldown();
            }
        }

        //currentReloadingTimer -= Time.deltaTime;
    }

    protected virtual void UltimateSkillUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space) && ultimateEnerge >= maxUltimateEnerge)
        {
            ultimateEnerge = 0.0f;

            Status selfStatus = null;

            if (ultimateSkill.SelfStatus)
            {
                selfStatus = ultimateSkill.SelfStatus;
                selfStatus.modifyPowerRate = stats.Calculate_StatusPower();
                selfStatus.modifyDurationRate = stats.Calculate_StatusDuration();
                selfStatus.stats = stats;
                player.StatusManager.AddStatus(selfStatus);
            }

            Vector3 aimDirection = aim.aimDirection;

            if (ultimateSkill.DamageCollider)
            {
                string poolKey = ultimateSkill.DamageCollider.name;
                GameObject damageColliderObj = PoolManager.Instance.GetObject(poolKey, ultimateSkill.DamageCollider.gameObject);
                DamageCollider damageCollider = damageColliderObj.GetComponent<DamageCollider>();
                damageCollider.Activate(ultSkill_InitPosition.position, Quaternion.Euler(0.0f, 0.0f, 0.0f));

                damageCollider.owner = player;

                if (selfStatus != null)
                {
                    damageCollider.ownerStatus = selfStatus;
                }

                if (damageCollider.GetComponentInChildren<SpriteRenderer>())
                {
                    var sprite = damageCollider.GetComponentInChildren<SpriteRenderer>();
                    float angle = Vector2.SignedAngle(new Vector2(1.0f, 0.0f), aimDirection);

                    switch (ultimateSkill.GetRenderMode)
                    {
                        case Skill.RenderMode.HorizontalFlip:
                            sprite.transform.localEulerAngles = new Vector3(0.0f, 0.0f, angle);

                            if (aimDirection.x < 0.0f)
                            {
                                sprite.transform.localScale = new Vector3(sprite.transform.localScale.x, -sprite.transform.localScale.y, sprite.transform.localScale.z);
                            }
                            break;
                        case Skill.RenderMode.AllFlip:
                            sprite.transform.localEulerAngles = new Vector3(0.0f, 0.0f, angle);

                            if (aimDirection.x < 0.0f)
                            {
                                sprite.transform.localScale = new Vector3(-sprite.transform.localScale.x, -sprite.transform.localScale.y, sprite.transform.localScale.z);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            player.BlowForceVelocity = aimDirection * ultimateSkill.SelfBlowForce;
        }
    }

    public void AttackChargeEnerge(float amount, float boostRate = 1.0f)
    {
        if (statusManager.IsChargeBanned())
            return;

        ultimateEnerge = Mathf.Min(ultimateEnerge + AttackEnergeFomula(amount) * boostRate, maxUltimateEnerge);
    }

    public void InjuryChargeEnerge(float amount, float boostRate = 1.0f)
    {
        if (statusManager.IsChargeBanned())
            return;

        ultimateEnerge = Mathf.Min(ultimateEnerge + amount * boostRate, maxUltimateEnerge);
    }

    public float AttackEnergeFomula(float amount)
    {
        return (amount * 0.1f);
    }

    protected void StartEnergeCharge()
    {
        if (energeChargeCoroutine != null)
        {
            StopCoroutine(energeChargeCoroutine);
        }
        energeChargeCoroutine = StartCoroutine(EnergeChargeOverTime());
    }

    private IEnumerator EnergeChargeOverTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(1.0f);

            if (!statusManager.IsChargeBanned() && ultimateEnerge < maxUltimateEnerge)
            {
                ultimateEnerge += stats.Calculate_AutoCharge();
                ultimateEnerge = Mathf.Min(ultimateEnerge, maxUltimateEnerge);
            }
        }
    }
}

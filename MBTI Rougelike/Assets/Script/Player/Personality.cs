using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.Windows;

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

    [Tooltip("普攻的生成位置。")]
    public Transform normalAttack_InitPosition;

    private bool isNormalAttackCharging = false;
    private float chargingTimer1 = 0.0f;
    private float chargingRate1 = 0.0f;

    [Header("特殊技能（特技）")]
    [SerializeField, Tooltip("该技能的数据。")]
    private Skill specialSkill;

    [SerializeField, Tooltip("该技能的当前冷却计时器。不大于0才能使用。")]
    private float specialSkill_CurretReloadingTimer;

    [Tooltip("特技的生成位置。")]
    public Transform specialSkill_InitPosition;

    private bool isSpecialSkillCharging = false;
    private float chargingTimer2 = 0.0f;
    private float chargingRate2 = 0.0f;


    [Header("终极技能（大招）")]
    [SerializeField, Tooltip("该大招能生成的伤害块")]
    private Skill ultimateSkill;

    [SerializeField, Tooltip("当前大招充能。充满到100才能释放大招，并消耗所有能量。")]
    private float ultimateEnerge = 0.0f;

    [Tooltip("大招的生成位置。")]
    public Transform ultSkill_InitPosition;

    [SerializeField, Tooltip("该大招能生成的伤害块")]
    private float maxUltimateEnerge = 100.0f;

    [SerializeField, Tooltip("某些非常规技能的特殊逻辑实现方法")]
    private PersonalitySpecialImplementation ultSpecialImplementation;


    [Header("互动组件")]
    [Tooltip("人格八维数据。")]
    public Preference preference;

    [Tooltip("能力值数据。")]
    public Stats stats;

    [Tooltip("状态管理机。")]
    public StatusManager statusManager;

    [Tooltip("固定位置生成的伤害块的的退回Offset，防止生成的【伤害块】过于靠前")]
    public float adjustBackOffset = 3.0f;

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

    public float ChargingRate1
    {
        get { return chargingRate1; }
        set { chargingRate1 = value; }
    }

    public float ChargingRate2
    {
        get { return chargingRate2; }
        set { chargingRate2 = value; }
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
            HandleSkillTypeAndControlScheme(normalAttack, ref normalAttack_CurretReloadingTimer, normalAttack_InitPosition, UnityEngine.Input.GetMouseButton(0), true); //左键
            HandleSkillTypeAndControlScheme(specialSkill, ref specialSkill_CurretReloadingTimer, specialSkill_InitPosition, UnityEngine.Input.GetMouseButton(1), false); //右键

            HandleUltiamteTypeAndControlScheme();
        }
    }

    protected virtual void SkillUpdate(Skill skill, ref float currentReloadingTimer, Transform initPos, bool holding, bool isAuto, float chargingRate = 1.0f)
    {
        Vector3 mousePos = UnityEngine.Input.mousePosition;
        var mouseWorldPos = Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
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
                    AttackHelper.InitSkillDamageCollider(skill, initPos, chargingRate, player, adjustBackOffset, aimDirection, scatterAngle);
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
        
            var sprite = damageCollider.GetComponentInChildren<SpriteRenderer>();
            if (sprite != null)
            {
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
                    case Skill.RenderMode.Lock:
                        sprite.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                        break;
                    default:
                        break;
                }
        
                var collider = damageCollider.GetComponentInChildren<Collider2D>();
                if (collider != null)
                {
                    collider.transform.localEulerAngles = sprite.transform.localEulerAngles;
                }
            }
        }
        
        player.BlowForceVelocity = aimDirection * ultimateSkill.SelfBlowForce;
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

    private void HandleSkillTypeAndControlScheme(Skill skill, ref float currentReloadingTimer, Transform initPos, bool input, bool isAuto)
    {
        switch (skill.SkillType)
        {
            case SkillCreateType.DamageCollider:

                switch (skill.ControlScheme)
                {
                    case SkillControlScheme.Continuous:
                        SkillUpdate(skill, ref currentReloadingTimer, initPos, input, isAuto);
                        break;
                    case SkillControlScheme.ChargeRelease:
                        if (isAuto)
                            ChargeReleaseUpdate(skill, ref currentReloadingTimer, initPos, input, isAuto, ref chargingTimer1, ref chargingRate1, ref isNormalAttackCharging);
                        else
                            ChargeReleaseUpdate(skill, ref currentReloadingTimer, initPos, input, isAuto, ref chargingTimer2, ref chargingRate2, ref isSpecialSkillCharging);
                        break;
                    case SkillControlScheme.Toggle:
                        if (input && currentReloadingTimer <= 0.0f)
                        {
                            SkillUpdate(skill, ref currentReloadingTimer, initPos, true, isAuto);
                        }
                        break;
                    default:
                        break;
                }
                break;
            case SkillCreateType.Turret:
                SpawnTurret(skill, ref currentReloadingTimer, initPos, input, isAuto);
                break;
            default:
                break;
        }
    }

    private void HandleUltiamteTypeAndControlScheme()
    {
        if (UnityEngine.Input.GetKeyDown(KeyCode.Space) && ultimateEnerge >= maxUltimateEnerge)
        {
            // 判定大招是否为特殊逻辑实现
            if (ultSpecialImplementation)
            {
                ultSpecialImplementation.ExecuteSpecialImplementation(this);
            }
            else
            {
                UltimateSkillUpdate();
            }

            ultimateEnerge = 0.0f;
        }
    }

    private void SkillChargingRateUpdate(DamageCollider damageCollider, float chargingRate)
    {
        damageCollider.damage += (int)(damageCollider.ChargingDamage * chargingRate);

        damageCollider.BlowForceSpeed += damageCollider.ChargingBlowForceSpeed * chargingRate;

        //damageCollider.Velocity = damageCollider.Velocity * chargingRate;

        //damageCollider.MaxTimer = damageCollider.MaxTimer * chargingRate;
        //damageCollider.Timer = damageCollider.MaxTimer;

        damageCollider.StaggerTime += damageCollider.ChargingStaggerTime * chargingRate;

        switch (damageCollider.damageMovementType)
        {
            case DamageCollider.DamageMovementType.Passive:
                damageCollider.spriteRenderer.transform.localScale += damageCollider.ChargingLocalScale * chargingRate;
                break;
            case DamageCollider.DamageMovementType.Projectile:
                damageCollider.MaxTimer += damageCollider.ChargingMaxTimer * chargingRate;
                damageCollider.Timer = damageCollider.MaxTimer;
                break;
            default:
                break;
        }
    }

    private void ChargeReleaseUpdate(Skill skill, ref float currentReloadingTimer, Transform initPos, bool input, bool isAuto, ref float chargingTimer, ref float chargingRate, ref bool isCharging)
    {
        if (input && currentReloadingTimer <= 0.0f && !isCharging)
        {
            // Start charging
            isCharging = true;
            chargingTimer = 0.0f;
        }
        else if (!input && isCharging)
        {
            // Release attack
            SkillUpdate(skill, ref currentReloadingTimer, initPos, true, isAuto, chargingRate);
            isCharging = false;
            chargingTimer = 0.0f;
            chargingRate = 0.0f;
        }
        else if (isCharging)
        {
            chargingTimer += Time.deltaTime;
            chargingRate = Mathf.Clamp(chargingTimer / skill.MaxChargingTime, 0.1f, 1.0f); // 0.1f是最低充能比率，1.0f是最高充能比率。
        }
    }

    private void SpawnTurret(Skill skill, ref float currReloadingTimer, Transform initPos, bool input, bool isAuto)
    {
        if (input && currReloadingTimer <= 0.0f)
        {
            string poolKey = skill.Turret.name;
            GameObject turretObj = PoolManager.Instance.GetObject(poolKey, skill.Turret.gameObject);

            Turret turret = turretObj.GetComponent<Turret>();

            turret.Activate(initPos.position, Quaternion.Euler(0.0f, 0.0f, 0.0f));

            // 重置冷却时间
            currReloadingTimer = skill.ReloadingTime;
            if (isAuto)
            {
                currReloadingTimer *= stats.Calculate_AttackSpeed();
            }
            else
            {
                currReloadingTimer *= stats.Calculate_SpecialCooldown();
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEditor;
using UnityEngine;

public class ParabolaDamageCollider : DamageCollider
{
    [SerializeField, Tooltip("初始向上速度。")]
    private float initialUpwardVelocity;

    [SerializeField, Tooltip("当前向上速度。")]
    private float currentUpwardVelocity;

    [SerializeField, Tooltip("模拟重力。")]
    private float gravity = -9.8f;

    [SerializeField, Tooltip("落地点。")]
    private Vector3 targetPosition;

    [SerializeField, Tooltip("初始位置。")]
    private Vector3 startPosition;

    [SerializeField, Tooltip("水平速度。")]
    private Vector3 horizontalVelocity;

    [SerializeField, Tooltip("该伤害块【落地】时播放的动画。")]
    private AnimationController2D landingEffectPrefab;

    [SerializeField, Tooltip("该【伤害块】是否由【鼠标】指出【落地点】。")]
    public bool mouseGuide = true; // 是否使用鼠标指引

    [SerializeField, Tooltip("该【伤害块】落地时生成的【伤害块】")]
    private DamageCollider landingDamageCollider;

    Player player;

    public Vector3 inputTargetPosition;
    public Vector3 InputTargetPosition { get => inputTargetPosition; set => inputTargetPosition = value; }


    protected override void OnStart()
    {
        base.OnStart();

        //timer = maxTimer;
        player = FindObjectOfType<Player>();
        InitializeParabola();
    }

    protected override void OnUpdate()
    {
        if (timer - Time.deltaTime < 0.0f)
        {
            OnLanding();
        }
    }

    protected override void FixedUpdate()
    {
        if (!isAttachedPos)
        {
            // 更新向上速度
            currentUpwardVelocity += gravity * Time.fixedDeltaTime;

            // 计算总速度
            Vector3 totalVelocity = horizontalVelocity + new Vector3(0, currentUpwardVelocity, 0);

            // 更新位置
            transform.Translate(totalVelocity * Time.fixedDeltaTime);
        }
        else
        {
            if (isAttachedPos && owner)
            {
                Vector3 fixInitPos = initPos;
                transform.position = owner.transform.position + initInterval;
            }
        }
        OnFixedUpdate();
    }

    private void OnLanding()
    {
        //// 处理落地后的逻辑，例如销毁对象或触发其他效果

        if (landingEffectPrefab) // 用于播放额外的落地特效
        {
            GameObject hitEffect = PoolManager.Instance.GetObject(landingEffectPrefab.name, landingEffectPrefab.gameObject);
            AnimationController2D anim = hitEffect.GetComponent<AnimationController2D>();
            anim.Activate(transform.position, Quaternion.identity);
        }

        AttackHelper.InitDamageCollider(landingDamageCollider, transform, 0.0f, Vector3.right, 0.0f, SkillControlScheme.None, false, 1.0f, Skill.RenderMode.Lock, player, 0.0f, owner);

        CreateSurface();
    }

    public void InitializeParabola()
    {
        startPosition = transform.position;

        if (mouseGuide)
        {
            targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPosition.z = 0;
        }
        else
        {
            targetPosition = InputTargetPosition;
        }

        // 计算水平速度
        Vector3 distance = targetPosition - startPosition;
        horizontalVelocity = new Vector3(distance.x / timer, 0, 0);

        // 计算初始向上速度
        initialUpwardVelocity = (distance.y - 0.5f * gravity * timer * timer) / timer;
        currentUpwardVelocity = initialUpwardVelocity;

        colliderActive = false;
    }

    public override void Activate(Vector3 position, Quaternion rotation)
    {
        base.Activate(position, rotation);
        InitializeParabola();
    }
}
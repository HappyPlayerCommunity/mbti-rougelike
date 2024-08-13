using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
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

    [SerializeField, Tooltip("落地时的伤害范围半径。")]
    private float landingDamageRadius = 2.0f;

    [SerializeField, Tooltip("该伤害块【落地】时播放的动画。")]
    private AnimationController2D landingEffectPrefab;

    protected override void OnStart()
    {
        base.OnStart();

        //timer = maxTimer;

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
        // 处理落地后的逻辑，例如销毁对象或触发其他效果

        //抛物体仅在落地时形成碰撞判定。
        colliderActive = true;

        // 扩大 collider 的范围
        float originalRadius = damageCollider2D.bounds.extents.x;
        damageCollider2D.transform.localScale *= landingDamageRadius / originalRadius;

        if (landingEffectPrefab)
        {
            GameObject hitEffect = PoolManager.Instance.GetObject(landingEffectPrefab.name, landingEffectPrefab.gameObject);
            AnimationController2D anim = hitEffect.GetComponent<AnimationController2D>();
            anim.Activate(transform.position, Quaternion.identity);
        }
        // 手动触发 OnTriggerEnter2D
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, landingDamageRadius);
        foreach (var hit in hits)
        {
            OnTriggerEnter2D(hit);
        }

        // 手动触发 CollisionCheck
        CollisionCheck();

        foreach (var hit in hits)
        {
            OnTriggerExit2D(hit);
        }

        damageCollider2D.transform.localScale /= landingDamageRadius / originalRadius;

        if (surface)
        {
            GameObject newSurfaceObj = PoolManager.Instance.GetObject(surface.name, surface.gameObject);
            Surface newSurface = newSurfaceObj.GetComponent<Surface>();
            newSurface.Activate(transform.position, Quaternion.identity);
        }
    }

    private void InitializeParabola()
    {
        startPosition = transform.position;
        targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        targetPosition.z = 0;

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

    ///// <summary>
    ///// 调用这个方法将对象塞回对象池。
    ///// </summary>
    //public override void Deactivate()
    //{
    //    OnLanding();

    //    base.Deactivate();
    //}

}
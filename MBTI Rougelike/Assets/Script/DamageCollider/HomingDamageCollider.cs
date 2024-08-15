using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingDamageCollider : DamageCollider
{
    [SerializeField, Tooltip("追踪敌人的速度。")]
    private float homingSpeed = 5.0f;

    [SerializeField, Tooltip("追踪敌人的范围。")]
    private float homingRange = 10.0f;

    [SerializeField, Tooltip("追踪延迟时间。")]
    private float homingDelay = 1.0f;

    [SerializeField, Tooltip("转向速度。")]
    private float turnSpeed = 5.0f;

    private Transform target;
    private bool isHomingActive = false;

    protected Coroutine activateHomingAfterDelayCoroutine;

    public float refindTime = 3.0f;

    public float refindTimer = 0.0f;


    protected override void Start()
    {
        base.Start();
        // 启动协程延迟追踪
        activateHomingAfterDelayCoroutine = StartCoroutine(ActivateHomingAfterDelay());
    }

    protected override void OnUpdate()
    {
        refindTimer -= Time.deltaTime;
        if (refindTimer <= 0.0f)
        {
            FindNearestTarget();
        }
    }

    protected override void FixedUpdate()
    {
        if (isHomingActive && target != null)
        {
            // 计算方向并移动
            Vector3 direction = (target.position - transform.position).normalized;
            Vector3 newDirection = Vector3.Lerp(transform.up, direction, turnSpeed * Time.fixedDeltaTime).normalized;
            transform.up = newDirection;
            transform.Translate(newDirection * homingSpeed * Time.fixedDeltaTime, Space.World);
        }
        else
        {
            // 如果没有目标，使用基础类的移动逻辑
            base.FixedUpdate();
        }

        OnFixedUpdate();
    }

    private void FindNearestTarget()
    {
        List<GameObject> targets = new List<GameObject>();
        if (!isHealingMode)
        {
            var enemies = GameObject.FindGameObjectsWithTag(Tag.Enemy);

            foreach (var enemy in enemies)
            {
                targets.Add(enemy);
            }

        }
        else
        {
            var bondes = GameObject.FindGameObjectsWithTag(Tag.Bond);
            foreach (var bond in bondes)
            {
                targets.Add(bond);
            }

            targets.Add(GameObject.FindGameObjectWithTag(Tag.Player));
        }

        float nearestDistance = homingRange;
        Transform nearestTarget = null;

        foreach (GameObject targetObj in targets)
        {
            float distance = Vector3.Distance(transform.position, targetObj.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestTarget = targetObj.transform;
            }
        }

        target = nearestTarget;

        refindTimer = refindTime;
    }

    private IEnumerator ActivateHomingAfterDelay()
    {
        // 等待指定的延迟时间
        yield return new WaitForSeconds(homingDelay);
        // 激活追踪
        isHomingActive = true;

        FindNearestTarget();
    }

    /// <summary>
    /// 当对象从对象池中取出时，调用这个方法来初始化。
    /// </summary>
    public override void Activate(Vector3 position, Quaternion rotation)
    {
        base.Activate(position, rotation);
        // 重置追踪状态
        isHomingActive = false;
        isHealingMode = false;

        if (gameObject.activeInHierarchy) {
            activateHomingAfterDelayCoroutine = StartCoroutine(ActivateHomingAfterDelay());
        }
    }

    /// <summary>
    /// 调用这个方法将对象塞回对象池。
    /// </summary>
    public override void Deactivate()
    {
        base.Deactivate();
        isHealingMode = false;
        isHomingActive = false;
        StopCoroutine(ActivateHomingAfterDelay());
    }
}

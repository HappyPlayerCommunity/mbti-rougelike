using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


// 此逻辑只对非玩家生效。如果要对玩家生效，需要添加不同的分支代码。因为玩家数据的结算和其他单位不同。
[CreateAssetMenu(fileName = "NewSlimeStatus", menuName = "Status Data/Slime Status")]
public class SlimeStatus : Status
{
    public float slowRate = 0.9f;
    private GameObject slowTarget;

    public override void OnUpdate(GameObject target, float deltaTime)
    {
        base.OnUpdate(target, deltaTime);
    }

    public override void OnApply(GameObject target)
    {
        base.OnApply(target);
        ApplySlow(target, slowRate);
        slowTarget = target;
    }

    public override void OnStack(Status status)
    {
        base.OnStack(status);
        SlimeStatus slimeStatus = (SlimeStatus)status;

        RemoveSlow(slowTarget, slowRate);

        slowRate *= slimeStatus.slowRate;

        ApplySlow(slowTarget, slowRate);
    }

    public override void OnExpire(GameObject target)
    {
        base.OnExpire(target);
        RemoveSlow(target, slowRate);
    }

    private void ApplySlow(GameObject target, float rate)
    {
        BaseEntity entity = target.GetComponent<BaseEntity>();
        if (entity != null)
        {
            entity.MovementSpeed *= rate * (1.0f / modifyPowerRate);
        }
    }

    private void RemoveSlow(GameObject target, float rate)
    {
        BaseEntity entity = target.GetComponent<BaseEntity>();
        if (entity != null)
        {
            entity.MovementSpeed /= rate * (1.0f / modifyPowerRate);
        }
    }
}

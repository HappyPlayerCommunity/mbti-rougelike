using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewENFJStatus", menuName = "Status Data/ENFJ Ult Data")]
public class StatusEnfjUlt : Status
{
    public override void OnUpdate(GameObject target, float deltaTime)
    {
        base.OnUpdate(target, deltaTime);
    }

    public override void OnApply(GameObject target)
    {
       base.OnApply(target);
    }

    public override void OnExpire(GameObject target)
    {
        base.OnExpire(target);
    }
}

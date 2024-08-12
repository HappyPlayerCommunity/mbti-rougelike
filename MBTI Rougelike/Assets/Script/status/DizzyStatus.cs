using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDizzyStatus", menuName = "Status Data/Dizzy Data")]
public class DizzyStatus : Status
{
    public float physicalVulnerable = 1.0f; // 后续或许可以添加物理倍率？

    public override void OnUpdate(GameObject target, float deltaTime)
    {
        target.GetComponentInChildren<SpriteRenderer>().color = Color.gray;
        base.OnUpdate(target, deltaTime);
    }

    public override void OnApply(GameObject target)
    {
        base.OnApply(target);
        PlayAnimation(target);
    }

    public override void OnExpire(GameObject target)
    {
        base.OnExpire(target);
        recordAnim.OnAnimationEnd();
    }
}

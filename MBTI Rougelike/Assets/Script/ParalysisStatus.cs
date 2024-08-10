using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

[CreateAssetMenu(fileName = "NewParalysisStatus", menuName = "Status Data/Paralysis Data")]
public class ParalysisStatus : Status
{
    public float thunderVulnerable = 1.0f; // 后续添加弱雷倍率。

    public override void OnUpdate(GameObject target, float deltaTime)
    {
        //临时标识，后续添加特效。
        target.GetComponentInChildren<SpriteRenderer>().color = Color.yellow;
        base.OnUpdate(target, deltaTime);
    }

    public override void OnApply(GameObject target)
    {
        base.OnApply(target);
    }

    public override void OnExpire(GameObject target)
    {
        target.GetComponentInChildren<SpriteRenderer>().color = Color.white;
        base.OnExpire(target);
    }
}

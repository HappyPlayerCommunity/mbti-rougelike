using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewIsfpExpandStatus", menuName = "Status Data/IsfpExpand Data")]
public class IsfpExpandStatus : Status
{
    public float expansionFactor = 3.0f; // 扩大倍数

    public override void OnApply(GameObject target)
    {
        base.OnApply(target);

        // 获取 SurfaceEffectManager 的实例
        SurfaceEffectManager surfaceEffectManager = SurfaceEffectManager.Instance;
        if (surfaceEffectManager == null)
        {
            Debug.LogError("SurfaceEffectManager instance not found.");
            return;
        }

        var personality = target.GetComponent<Personality>();
        Transform transform = personality.specialSkill_InitPosition;

        // 获取当前 GameObject 上优先级最高的地表
        Surface highestPrioritySurface = surfaceEffectManager.FindHighestPrioritySurface(transform.gameObject);
        if (highestPrioritySurface == null)
        {
            if (personality != null) {
                Debug.Log("目前没有地表。");
                return;
            }
        }

        highestPrioritySurface.durationTimer = highestPrioritySurface.duration;

        // 扩大地表
        ExpandSurface(highestPrioritySurface);
        surfaceEffectManager.SetToHighPriority(highestPrioritySurface);
    }

    private void ExpandSurface(Surface surface)
    {
        // 获取地表的 Transform 组件
        Transform surfaceTransform = surface.transform;

        // 扩大地表
        surfaceTransform.localScale *= expansionFactor;
    }
}

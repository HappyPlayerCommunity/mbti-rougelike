using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusManager : MonoBehaviour
{
    [SerializeField, Tooltip("当前激活的所有状态。")]
    private List<Status> activeStatus = new List<Status>();

    [SerializeField, Tooltip("禁足标识数。如果此数值大于0，则玩家无法移动。")]
    int rootCount = 0;

    [SerializeField, Tooltip("禁止充能标识数。如果此数值大于0，则玩家无法充能。")]
    int chargeBanCount = 0;

    [SerializeField, Tooltip("无敌标识数。如果此数值大于0，则玩家会受到任何伤害。")]
    int invincibleCount = 0;

    [SerializeField, Tooltip("沉默标识数。如果此数值大于0，则玩家无法释放任何技能。")]
    int silenceCount = 0;

    void Update()
    {
        float deltaTime = Time.deltaTime;
        for (int i = activeStatus.Count - 1; i >= 0; i--)
        {
            activeStatus[i].OnUpdate(gameObject, deltaTime);
            if (activeStatus[i].IsExpired())
            {
                activeStatus[i].OnExpire(gameObject);
                RemoveFlagUpdate(activeStatus[i]);
                activeStatus.RemoveAt(i);
            }
        }
    }

    public void AddStatus(Status status)
    {
        //需要检测不能是否是同样的buff类型，且如何结算。

        AddFlagUpdate(status);

        status.OnApply(gameObject);

        activeStatus.Add(status);
    }

    /// <summary>
    /// 手动移除状态，而非自然结束。
    /// </summary>
    /// <param name="status"></param>
    public void RemoveStatus(Status status) 
    {
        RemoveFlagUpdate(status);

        status.OnExpire(gameObject);
        activeStatus.Remove(status);
    }

    public bool IsRooted()
    {
        return rootCount > 0;
    }

    public bool IsChargeBanned()
    {
        return chargeBanCount > 0;
    }

    public bool IsInvincible()
    {
        return invincibleCount > 0;
    }

    public bool IsSlienced()
    {
        return silenceCount > 0;
    }

    public void AddFlagUpdate(Status status)
    {
        if (status.root)
            rootCount += 1;

        if (status.ultChargeBan)
            chargeBanCount += 1;

        if (status.invincible)
            invincibleCount += 1;

        if (status.silence)
            silenceCount += 1;
    }

    public void RemoveFlagUpdate(Status status)
    {
        if (status.root)
            rootCount -= 1;

        if (status.ultChargeBan)
            chargeBanCount -= 1;

        if (status.invincible)
            invincibleCount -= 1;

        if (status.silence)
            silenceCount -= 1;
    }
}

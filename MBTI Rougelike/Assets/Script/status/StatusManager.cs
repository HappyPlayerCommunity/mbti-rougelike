using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusManager : MonoBehaviour
{
    [SerializeField, Tooltip("当前激活的所有状态。")]
    private List<Status> activeStatuses = new List<Status>();

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

        // 需要考虑其他线程的状态更新，需要复制一份状态列表。
        List<Status> statusesCopy = new List<Status>(activeStatuses);

        foreach (var status in statusesCopy)
        {
            status.OnUpdate(gameObject, deltaTime);
            if (status.IsExpired())
            {
                status.OnExpire(gameObject);
                RemoveFlagUpdate(status);
                activeStatuses.Remove(status);
            }
        }
    }

    public Status AddStatus(Status newStatus, Stats stats)
    {
        Status existingStatus = activeStatuses.Find(status => status.GetType() == newStatus.GetType());

        if (stats)
        {
            newStatus.modifyPowerRate = stats.Calculate_StatusPower();
            newStatus.modifyImpactRate = stats.Calculate_StatusImpact();
            newStatus.modifyDurationRate = stats.Calculate_StatusDuration();
            newStatus.stats = stats;
        }

        if (existingStatus) //已有同类状态，执行叠加逻辑。
        {
            existingStatus.OnStack(newStatus);
            existingStatus.stats = stats;

            return existingStatus;
        }
        else // 新状态
        {

            //Debug.Log("before activeStatuses" + activeStatuses[0]);

            Status finalStatus = Instantiate(newStatus);

            finalStatus.OnApply(gameObject);
            AddFlagUpdate(finalStatus);
            activeStatuses.Add(finalStatus);

            finalStatus.stats = stats;

            return finalStatus;
        }
    }

    /// <summary>
    /// 手动移除状态，而非自然结束。
    /// </summary>
    /// <param name="status"></param>
    public void RemoveStatus(Status status) 
    {
        RemoveFlagUpdate(status);

        status.OnExpire(gameObject);
        activeStatuses.Remove(status);
    }

    public void RemoveAllStatus()
    {
        List<Status> statusesToRemove = new List<Status>(activeStatuses);

        foreach (var status in statusesToRemove)
        {
            status.OnExpire(gameObject);
            RemoveFlagUpdate(status);
        }

        activeStatuses.Clear(); // 清空所有状态
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

    public bool IsStunned()
    {
        return false;
    }

    public List<Status> ActiveStatuses()
    {
        return activeStatuses;
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

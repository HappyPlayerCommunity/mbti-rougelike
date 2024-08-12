using System.Collections.Generic;
using UnityEngine;

public abstract class Surface : MonoBehaviour
{
    // 持续时间，-1表示无限时长
    public float duration = -1f;

    // 用于存储标签与效果处理方法的映射
    protected Dictionary<string, System.Action<GameObject>> tagEffectMap;
    protected Dictionary<string, System.Action<GameObject>> updateEffectMap;
    protected Dictionary<string, System.Action<GameObject>> removeEffectMap;

    // 存储当前站在地表上的单位
    protected List<GameObject> unitsOnSurface = new List<GameObject>();

    protected virtual void Start()
    {
        InitializeTagEffectMap();
        InitializeRemoveEffectMap();
        InitializeUpdateEffectMap();
    }

    protected virtual void InitializeTagEffectMap()
    {
        tagEffectMap = new Dictionary<string, System.Action<GameObject>>();

        tagEffectMap.Add(Tag.Enemy, unit => { });
        tagEffectMap.Add(Tag.Bond, unit => { });
        tagEffectMap.Add(Tag.Player, unit => { });
    }

    protected virtual void InitializeRemoveEffectMap()
    {
        removeEffectMap = new Dictionary<string, System.Action<GameObject>>();

        removeEffectMap.Add(Tag.Enemy, unit => { });
        removeEffectMap.Add(Tag.Bond, unit => { });
        removeEffectMap.Add(Tag.Player, unit => { });
    }

    protected virtual void InitializeUpdateEffectMap()
    {
        updateEffectMap = new Dictionary<string, System.Action<GameObject>>();

        updateEffectMap.Add(Tag.Enemy, unit => { });
        updateEffectMap.Add(Tag.Bond, unit => { });
        updateEffectMap.Add(Tag.Player, unit => { });
    }

    // 应用效果的方法
    protected virtual void ApplyEffect(GameObject unit)
    {
        string unitTag = unit.tag;
        if (tagEffectMap.ContainsKey(unitTag))
        {
            tagEffectMap[unitTag].Invoke(unit);
        }
    }

    // 移除效果的方法
    protected virtual void RemoveEffect(GameObject unit)
    {
        string unitTag = unit.tag;
        if (removeEffectMap.ContainsKey(unitTag))
        {
            removeEffectMap[unitTag].Invoke(unit);
        }
    }

    // 更新地表效果的方法
    protected virtual void UpdateSurface(GameObject unit)
    {
        string unitTag = unit.tag;
        if (updateEffectMap.ContainsKey(unitTag))
        {
            updateEffectMap[unitTag].Invoke(unit);
        }
    }

    // Unity的Update方法，用于处理持续时间和清理
    protected virtual void Update()
    {
        if (duration > 0.0f)
        {
            duration -= Time.deltaTime;
            if (duration <= 0.0f)
            {
                OnDurationEnd();
            }
        }

        foreach (var unit in unitsOnSurface)
        {
            UpdateSurface(unit);
        }
    }

    // 持续时间结束时调用
    protected virtual void OnDurationEnd()
    {
        Destroy(gameObject);
    }

    // 进入地表时调用的方法
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        GameObject unit = other.gameObject;
        ApplyEffect(unit);
        unitsOnSurface.Add(unit);
    }

    // 离开地表时调用的方法
    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        GameObject unit = other.gameObject;
        RemoveEffect(unit);
        unitsOnSurface.Remove(unit);
    }

    public void SetDuration(float newDuration)
    {
        duration = newDuration;
    }

    // 处理元素反应的方法
    public virtual void ReactToElement(DamageElementType element, GameObject source) { }
}
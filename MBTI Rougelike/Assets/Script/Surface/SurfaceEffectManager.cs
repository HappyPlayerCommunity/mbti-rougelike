using System.Collections.Generic;
using UnityEngine;

public class SurfaceEffectManager : MonoBehaviour
{
    private static SurfaceEffectManager _instance;
    private static bool isShuttingDown = false;
    public static SurfaceEffectManager Instance
    {
        get
        {
            if (isShuttingDown) return null;  // 如果正在关闭，直接返回 null

            if (_instance == null)
            {
                _instance = FindObjectOfType<SurfaceEffectManager>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject("SurfaceEffectManager");
                    _instance = obj.AddComponent<SurfaceEffectManager>();
                }
            }
            return _instance;
        }
    }

    public Dictionary<GameObject, Surface> activeSurfaceEffects = new Dictionary<GameObject, Surface>();

    private int currentMaxPriority = 0;  // 全局优先级计数器

    private void Awake()
    {
        //Debug.Log("SurfaceEffectManager Awake");
        if (_instance == null)
        {
            _instance = this;
            //Debug.Log("SurfaceEffectManager Initialized.");
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            //Debug.Log("SurfaceEffectManager Destroyed.");
            Destroy(gameObject);
        }
    }

    // 为新生成的地表分配优先级
    public void RegisterNewSurface(Surface surface)
    {
        currentMaxPriority++;
        surface.priority = currentMaxPriority;
    }

    public void ApplySurfaceEffect(GameObject obj, Surface newSurface)
    {
        if (activeSurfaceEffects.ContainsKey(obj))
        {
            Surface currentSurface = activeSurfaceEffects[obj];
            if (newSurface.priority > currentSurface.priority)
            {
                // 移除当前地表效果，应用新的地表效果
                currentSurface.RemoveEffect(obj);
                newSurface.ApplyEffect(obj);
                activeSurfaceEffects[obj] = newSurface;
                //Debug.Log("进入地形——更高的地形");
            }
        }
        else
        {
            // 如果没有其他地表效果，则直接应用新地表效果
            newSurface.ApplyEffect(obj);
            activeSurfaceEffects[obj] = newSurface;
            //Debug.Log("进入地形——上一层地形");
        }
    }

    public void RemoveSurfaceEffect(GameObject obj, Surface surface)
    {
        if (activeSurfaceEffects.ContainsKey(obj) && activeSurfaceEffects[obj] == surface)
        {
            surface.RemoveEffect(obj);
            activeSurfaceEffects.Remove(obj);

            // 检查是否还有其他重叠的地表，如果有则应用优先级最高的地表效果
            Surface newSurface = FindHighestPrioritySurface(obj);
            if (newSurface != null)
            {
                newSurface.ApplyEffect(obj);
                activeSurfaceEffects[obj] = newSurface;
                //Debug.Log("进入地形——下一层地形");
            }
        }
    }

    private Surface FindHighestPrioritySurface(GameObject unit)
    {
        Surface highestSurface = null;
        Collider2D[] overlappingSurfaces = Physics2D.OverlapPointAll(unit.transform.position);

        foreach (var collider in overlappingSurfaces)
        {
            Surface surface = collider.GetComponent<Surface>();
            if (surface != null)
            {
                if (highestSurface == null || surface.priority > highestSurface.priority)
                {
                    highestSurface = surface;
                }
            }
        }

        return highestSurface;
    }

    public bool IsUnderSurfaceEffect(GameObject obj, System.Type surfaceType)
    {
        return activeSurfaceEffects.ContainsKey(obj) && activeSurfaceEffects[obj].GetType() == surfaceType;
    }

    public List<GameObject> GetEntitiesAffectedBySurface(Surface surface)
    {
        List<GameObject> affectedUnits = new List<GameObject>();

        foreach (var kvp in activeSurfaceEffects)
        {
            if (kvp.Value == surface)
            {
                affectedUnits.Add(kvp.Key);
            }
        }

        return affectedUnits;
    }

    public void RemoveSurfaceEffectFromAllEntity(Surface surface)
    {
        List<GameObject> unitsToRemove = new List<GameObject>();

        // 找出所有受这个 Surface 影响的单位
        foreach (var kvp in activeSurfaceEffects)
        {
            if (kvp.Value == surface)
            {
                // 从单位中移除这个 Surface 的效果
                surface.RemoveEffect(kvp.Key);
                unitsToRemove.Add(kvp.Key);
            }
        }

        // 从字典中移除受影响的单位
        foreach (var unit in unitsToRemove)
        {
            activeSurfaceEffects.Remove(unit);
        }
    }

    public void RemoveEntityFromSurfaceEffects(GameObject obj)
    {
        if (activeSurfaceEffects.ContainsKey(obj))
        {
            // 找到并移除对该单位的引用
            Surface surface = activeSurfaceEffects[obj];
            surface?.RemoveEffect(obj);
            activeSurfaceEffects.Remove(obj);
        }
    }

    private void OnDestroy()
    {
        isShuttingDown = true;
        Debug.Log("SurfaceEffectManager Destroyed.");
        CleanupActiveSurfaceEffects();
        if (_instance == this)
        {
            _instance = null;
        }
    }

    private void OnDisable()
    {
        Debug.Log("SurfaceEffectManager Disable.");
        CleanupActiveSurfaceEffects();
        if (_instance == this)
        {
            _instance = null;
        }
    }

    public void CleanupActiveSurfaceEffects()
    {
        List<Surface> surfacesToCleanup = new List<Surface>(activeSurfaceEffects.Values);

        foreach (var surface in surfacesToCleanup)
        {
            if (surface != null)
            {
                surface.Deactivate();
            }
        }

        activeSurfaceEffects.Clear();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 对象池管理器类，用于管理和访问不同类型的对象池。
/// </summary>
public class PoolManager : MonoBehaviour
{
    private static PoolManager _instance;
    public static PoolManager Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject("PoolManager");
                _instance = go.AddComponent<PoolManager>();
            }
            return _instance;
        }
    }

    private Dictionary<string, ObjectPool> _pools = new Dictionary<string, ObjectPool>();

    private GameObject poolContainer;

    private void Awake()
    {
        //Debug.Log("PoolManager Awake.");

        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject); // 确保在场景切换时不会销毁
            poolContainer = new GameObject("ObjectPoolContainer");

            //Debug.Log("PoolManager Initialized.");

        }
        else if (_instance != this)
        {
            //Debug.Log("PoolManager Destroyed.");
            Destroy(gameObject);
        }
    }
    public ObjectPool GetPool(string key)
    {
        key = NormalizePoolKey(key);

        if (_pools.TryGetValue(key, out ObjectPool pool))
        {
            return pool;
        }
        return null;
    }

    public void CreatePool(string key, GameObject prefab, int initialSize)
    {
        key = NormalizePoolKey(key);
        //Debug.Log("Create key: " + key);

        if (!_pools.ContainsKey(key))
        {
            //Debug.Log("Pool created");
            ObjectPool pool = new ObjectPool(prefab, initialSize, poolContainer);
            _pools[key] = pool;
        }
    }

    public GameObject GetObject(string key, GameObject prefab, int initialSize = 10)
    {
        key = NormalizePoolKey(key);
        // Debug.Log("Get Object Key: " + key);

        if (!_pools.ContainsKey(key))
        {
            //Debug.Log("Create Pool" + key);
            CreatePool(key, prefab, initialSize);
        }
        else
        {
            //Debug.Log("不需要创建新的");
        }

        GameObject obj = _pools[key].Get();
        obj.transform.SetParent(poolContainer.transform);
        return obj;
    }

    public void ReturnObject(string key, GameObject obj)
    {
        key = NormalizePoolKey(key);

        if (_pools.TryGetValue(key, out ObjectPool pool))
        {
            //Debug.Log("Acutal Return");
            pool.Return(obj);
        }
    }

    public static string NormalizePoolKey(string name)
    {
        return name.Replace("(Clone)", "").Trim();
    }

    private void OnDisable()
    {
        Debug.Log("PoolManager Disable.");
        CleanupPools();
    }

    private void OnDestroy()
    {
        Debug.Log("PoolManager Destroyed.");
        CleanupPools();
    }

    private void CleanupPools()
    {
        // 遍历所有池并清空它们
        foreach (var pool in _pools.Values)
        {
            pool.Clear();
        }

        _pools.Clear();

        // 销毁池容器
        if (poolContainer != null)
        {
            Destroy(poolContainer);
        }
    }
}

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
        poolContainer = GameObject.Find("ObjectPoolContainer");
        if (poolContainer == null)
        {
            poolContainer = new GameObject("ObjectPoolContainer");
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

    private void Update()
    {
        //foreach (var kvp in _pools)
        //{
        //    Debug.Log($"Key: {kvp.Key}, Pool Size: {kvp.Value}");
        //}
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
}

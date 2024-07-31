using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 通用对象池类，用于管理特定类型的对象实例化和重用。
/// </summary>
public class ObjectPool
{
    private GameObject _prefab;
    private Queue<GameObject> _pool;
    private GameObject _poolContainer;

    public ObjectPool(GameObject prefab, int initialSize, GameObject poolContainer)
    {
        _poolContainer = poolContainer;
        _prefab = prefab;
        _pool = new Queue<GameObject>();

        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = Object.Instantiate(_prefab);
            obj.transform.SetParent(poolContainer.transform);
            obj.SetActive(false);
            _pool.Enqueue(obj);
        }
    }

    public GameObject Get()
    {
        GameObject obj;
        //Debug.Log("计数Count: " + _pool.Count);
        if (_pool.Count > 0)
        {
            obj = _pool.Dequeue();
            //obj.SetActive(true);
            ResetObjectState(obj);
            //Debug.Log("复用: " + PoolManager.NormalizePoolKey(obj.name));
        }
        else
        {
            obj = Object.Instantiate(_prefab);
            //Debug.Log("新物体: " + PoolManager.NormalizePoolKey(obj.name));
        }
        obj.SetActive(true);
        return obj;
    }

    public void Return(GameObject obj)
    {
        obj.SetActive(false);
        _pool.Enqueue(obj);
        //Debug.Log("返还: " + PoolManager.NormalizePoolKey(obj.name));

    }

    private void ResetObjectState(GameObject obj)
    {
        var poolable = obj.GetComponent<IPoolable>();
        if (poolable != null)
        {
            poolable.ResetObjectState();
        }
        else
        {
            Debug.LogWarning("该物体没有继承自IPoolable: " + obj.name);
        }
    }
}
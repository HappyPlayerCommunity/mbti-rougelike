using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 管理血量显示的类。目前就是简单地在各个单位头上顶一个血条。
/// </summary>
public class HPController : MonoBehaviour, IPoolable
{
    public Slider hp;
    public Slider shield;
    public BaseEntity baseEntity;
    private RectTransform rectTransform;
    public Vector3 offset = new Vector3(0.0f, 0.75f, 0.0f);
    private string poolKey;

    public string PoolKey
    {
        get { return poolKey; }
        set { poolKey = value; }
    }

    private void Awake()
    {
        poolKey = gameObject.name;
    }

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        hp.value = (float)baseEntity.HP / (float)baseEntity.MaxHP;

        if (baseEntity.Shield > 0)
        {
            // 护盾不会超过当前血条的长度。
            float shieldPercentage = (float)baseEntity.Shield / (float)baseEntity.MaxShield;

            if (baseEntity.MaxShield <= 0)
            {
                shieldPercentage = (float)baseEntity.Shield / (float)baseEntity.MaxHP;
            }
            float effectiveShieldPercentage = Mathf.Min(shieldPercentage, hp.value);

            shield.value = effectiveShieldPercentage;
        }
        else
        {
            shield.value = 0;
        }

        Vector3 screenPosition = Camera.main.WorldToScreenPoint(baseEntity.transform.position + offset);
        rectTransform.position = screenPosition;
    }

    private void HandleDeath()
    {
        //gameObject.SetActive(false);
        Deactivate();
    }

    private void HandleRespawn()
    {
        //gameObject.SetActive(true);
        ResetObjectState();
    }

    /// <summary>
    /// 继承自IPoolable接口的方法。用于对象池物体的初始化。
    /// </summary>
    public void ResetObjectState()
    {
        if (baseEntity != null)
        {
            hp.value = (float)baseEntity.HP / (float)baseEntity.MaxHP;
            if (baseEntity.MaxShield > 0)
            {
                float shieldPercentage = (float)baseEntity.Shield / (float)baseEntity.MaxShield;
                float effectiveShieldPercentage = Mathf.Min(shieldPercentage, hp.value);
                shield.value = effectiveShieldPercentage;
            }
            else
            {
                shield.value = 0;
            }
        }
    }

    /// <summary>
    /// 当对象从对象池中取出时，调用这个方法来初始化
    /// </summary>
    public void Activate(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;

        gameObject.SetActive(true);
    }

    /// <summary>
    /// 调用这个方法将对象塞回对象池
    /// </summary>
    public void Deactivate()
    {
        gameObject.SetActive(false);

        PoolManager.Instance.ReturnObject(poolKey, gameObject);
    }
}

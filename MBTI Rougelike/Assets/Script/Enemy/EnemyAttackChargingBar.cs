using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyAttackChargingBar : MonoBehaviour, IPoolable
{
    [SerializeField, Tooltip("充能条的图像组件。")]
    private Image chargingBarImage;

    [SerializeField, Tooltip("敌人对象。")]
    public Enemy enemy;

    public RectTransform rectTransform;

    public Vector3 offset;

    private string poolKey;

    private Color transparent = Color.white;

    public string PoolKey
    {
        get { return poolKey; }
        set { poolKey = value; }
    }

    private void Awake()
    {
        poolKey = gameObject.name;
    }

    void Start()
    {
        if (chargingBarImage == null)
        {
            Debug.LogError("Charging bar image is not assigned.");
        }

        if (enemy == null)
        {
            Debug.LogError("Enemy is not assigned.");
        }

        transparent.a = 0.0f;
    }

    void Update()
    {
        if (enemy != null && chargingBarImage != null)
        {
            UpdateChargingBar();
        }

        Vector3 screenPosition = Camera.main.WorldToScreenPoint(enemy.transform.position + offset);
        rectTransform.position = screenPosition;
    }

    /// <summary>
    /// 更新充能条的显示。
    /// </summary>
    private void UpdateChargingBar()
    {
        float ratio = enemy.IsPreparingAttack ? 1.0f - enemy.PreparationTimer / enemy.PreparationTime : 0.0f;
            
        chargingBarImage.fillAmount = Mathf.Clamp01(ratio);
        chargingBarImage.color = Color.Lerp(transparent, Color.red, chargingBarImage.fillAmount);
    }

    /// <summary>
    /// 继承自IPoolable接口的方法。用于对象池物体的初始化。
    /// </summary>
    public void ResetObjectState()
    {
        if (enemy != null)
        {
            float ratio = enemy.PreparationTimer / enemy.PreparationTime;
            chargingBarImage.fillAmount = Mathf.Clamp01(ratio);
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

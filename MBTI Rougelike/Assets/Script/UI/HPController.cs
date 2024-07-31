using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 管理血量显示的类。目前就是简单地在各个单位头上顶一个血条。
/// </summary>
public class HPController : MonoBehaviour
{
    public Slider hp;
    public Slider shield;
    public BaseEntity baseEntity;
    private RectTransform rectTransform;
    public Vector3 offset = new Vector3(0.0f, 0.75f, 0.0f);

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        if (baseEntity != null)
        {
            baseEntity.OnDeath += HandleDeath;
        }
    }

    private void Update()
    {
        if (baseEntity == null)
        {
            Destroy(gameObject);
            return;
        }

        if (!baseEntity.isActiveAndEnabled)
        {
            gameObject.SetActive(false);
        }

        hp.value = (float)baseEntity.HP / (float)baseEntity.MaxHP;

        if (baseEntity.MaxShield > 0 && baseEntity.Shield > 0)
        {
            // 护盾不会超过当前血条的长度。
            float shieldPercentage = (float)baseEntity.Shield / (float)baseEntity.MaxShield;
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
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (baseEntity != null)
        {
            baseEntity.OnDeath -= HandleDeath;
        }
    }
}

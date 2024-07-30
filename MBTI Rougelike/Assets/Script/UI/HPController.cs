using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 管理血量显示的类。目前就是简单地在各个单位头上顶一个血条。
/// </summary>
public class HPController : MonoBehaviour
{
    public Slider HP;
    public BaseEntity baseEntity;
    private RectTransform rectTransform;
    public Vector3 offset = new Vector3(0.0f, 0.75f, 0.0f);

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        HP = GetComponent<Slider>();

        if (baseEntity != null)
        {
            baseEntity.OnDeath += HandleDeath; // 订阅 OnDeath 事件
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

        HP.value = (float)baseEntity.HP / (float)baseEntity.MaxHP;

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum PopupType
{
    Damage = 0,
    Miss = 1,


    //  后续可以为破盾，元素反应，撞墙等情况添加新的弹出类型。
}

public class DamagePopupManager : MonoBehaviour
{
    public static DamagePopupManager Instance { get; private set; }

    public DamagePopup damagePopupPrefab;
    public Transform canvasTransform;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    /// <summary>
    /// 使用伤害弹出时，不要忘记在第三个参数中传入伤害值，不然默认为0。
    /// 
    /// 【闪避】，【破盾】等其他弹出，或许应该使用不同的预制件甚至类，来做到不同的效果。
    ///  但目前先统一，简单地归类为DamagePopup。某种程度上，它们也是和伤害相关的。
    /// </summary>
    public void Popup(PopupType popupType, Vector3 position, int damage = 0, bool isCrit = false)
    {
        GameObject popupObj = PoolManager.Instance.GetObject(damagePopupPrefab.name, damagePopupPrefab.gameObject);
        DamagePopup damagePopup = popupObj.GetComponent<DamagePopup>();
        damagePopup.Activate(position, Quaternion.identity);

        switch (popupType)
        {
            case PopupType.Damage:
                damagePopup.SetDamage(damage, isCrit);
                break;
            case PopupType.Miss:
                damagePopup.SetMiss();
                break;
            default:
                break;
        }

        damagePopup.transform.SetParent(canvasTransform, false);
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(position);
        damagePopup.GetComponent<RectTransform>().position = screenPosition;
    }
}

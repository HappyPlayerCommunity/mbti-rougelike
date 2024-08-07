using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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

    public void PopupDamageNumber(int damage, Vector3 position)
    {
        if (damagePopupPrefab)
        {
            GameObject popupObj = PoolManager.Instance.GetObject(damagePopupPrefab.name, damagePopupPrefab.gameObject);
            DamagePopup damagePopup = popupObj.GetComponent<DamagePopup>();
            damagePopup.Activate(position, Quaternion.identity);

            damagePopup.SetDamage(damage);
            damagePopup.transform.SetParent(canvasTransform, false);
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(position);
            damagePopup.GetComponent<RectTransform>().position = screenPosition;
        }
    }
}

using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CooldownController : MonoBehaviour
{
    public Image chargingBar;  // 环形充能条的Image
    public Personality personality;
    public RectTransform rectTransform;
    public Vector3 offset = Vector3.zero;

    private Color transparent = Color.white;
    private Color cyan = new Color(0.0f, 1.0f, 1.0f, 1.0f);

    [SerializeField, Tooltip("这是普攻的冷却条吗？不是的话就是特技的蓄能条。")]
    private bool isAuto = false;

    void Start()
    {
        chargingBar.fillAmount = 0.0f;
        if (personality == null)
        {
            personality = GameObject.FindObjectOfType<Personality>();
        }

        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }

        transparent.a = 0.0f;
    }

    void Update()
    {
        if (personality.player.IsAlive())
        {
            float cooldownRate = 0.0f;
            if (isAuto)
            {
                cooldownRate = 1.0f - Mathf.Clamp01(personality.NormalAttack_CurretReloadingTimer / personality.NormalAttack_CurretReloadingTime);
            }
            else
            {
                cooldownRate = 1.0f - Mathf.Clamp01(personality.SpecialSkill_CurretReloadingTimer / personality.SpecialSkill_CurretReloadingTime);
            }

            chargingBar.fillAmount = cooldownRate;

            Vector3 screenPosition = Camera.main.WorldToScreenPoint(personality.transform.position + offset);
            rectTransform.position = screenPosition;

            chargingBar.color = Color.Lerp(cyan, transparent, chargingBar.fillAmount);
        }
        else
        {
            chargingBar.color = Color.Lerp(cyan, transparent, 1.0f);
        }

    }
}

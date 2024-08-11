using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ChargingBarController : MonoBehaviour
{
    public Image chargingBar;  // 环形充能条的Image
    public Personality personality;
    public RectTransform rectTransform;
    public Vector3 offset = Vector3.zero;

    private Color transparent = Color.white;
    private Color orange = new Color(1.0f, 0.5f, 0.0f, 1.0f);

    [SerializeField, Tooltip("这是普攻的蓄能条吗？不是的话就是特技的蓄能条。")]
    private bool isAuto = false;

    void Start()
    {
        chargingBar.fillAmount = 0.0f;
        if (personality == null) {
            personality = GameObject.FindObjectOfType<Personality>();
        }

        if (rectTransform == null) { 
           rectTransform = GetComponent<RectTransform>();
        }

        transparent.a = 0.0f;
    }

    void Update()
    {
        if (personality.player.IsAlive())
        {
            if (isAuto)
                chargingBar.fillAmount = Mathf.Clamp01(personality.ChargingRate1);
            else
                chargingBar.fillAmount = Mathf.Clamp01(personality.ChargingRate2);

            Vector3 screenPosition = Camera.main.WorldToScreenPoint(personality.transform.position + offset);
            rectTransform.position = screenPosition;

            chargingBar.color = Color.Lerp(transparent, orange, chargingBar.fillAmount);
        }
        else
        {
            chargingBar.color = Color.Lerp(transparent, orange, 0.0f);
        }
    }
}
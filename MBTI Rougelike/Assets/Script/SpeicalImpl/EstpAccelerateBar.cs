using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EstpAccelerateBar : MonoBehaviour
{
    public Slider speed;
    public EstpController estpController;
    public Vector3 offset = Vector3.zero;
    public RectTransform rectTransform;
    private Image fillImage;
    private Image backgroundImage;

    void Start()
    {
        speed = GetComponent<Slider>();
        estpController = FindObjectOfType<EstpController>();

        fillImage = speed.fillRect.GetComponent<Image>();
        if (fillImage == null)
        {
            Debug.LogError("Fill Image component not found on the Slider.");
        }

        backgroundImage = speed.GetComponentInChildren<Image>();
        if (backgroundImage == null)
        {
            Debug.LogError("Background Image component not found on the Slider.");
        }

        if (!estpController)
        {
            gameObject.SetActive(false);
            Destroy(this);
        }
    }

    void Update()
    {
        if (!estpController || !estpController.gameObject.activeInHierarchy)
        {
            return;
        }

        float currentSpeed = estpController.currentSpeed;
        float maxSpeed = estpController.maxSpeed;
        float rate = currentSpeed / maxSpeed;

        speed.value = rate;

        Color fillColor = fillImage.color;
        fillColor.a = rate;
        fillImage.color = fillColor;

        Color backgroundColor = backgroundImage.color;
        backgroundColor.a = rate;
        backgroundImage.color = backgroundColor;
    }
}

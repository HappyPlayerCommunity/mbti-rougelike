using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DamagePopup : MonoBehaviour, IPoolable
{
    public TextMeshProUGUI damageText;
    public float lifetime = 1.0f;
    public float lifetimer = 0.0f;
    public float floatSpeed = 2.0f;

    public RectTransform rectTransform;
    public Vector3 renderPosition;

    public float outlineSize = 0.5f;
    public Color outlineColor = Color.black;
    private string poolKey;

    public Vector3 initLocalScale;
    public string missText = "Miss";

    public string PoolKey
    {
        get { return poolKey; }
        set { poolKey = value; }
    }

    void Awake()
    {
        poolKey = gameObject.name;
    }

    private void Start()
    {
        damageText = GetComponent<TextMeshProUGUI>();
        rectTransform = GetComponent<RectTransform>();

        initLocalScale = rectTransform.localScale;
        lifetimer = lifetime;
    }

    private void Update()
    {
        rectTransform.position += Vector3.up * floatSpeed * Time.deltaTime;

        float t = 1.0f - (lifetimer / lifetime);
        rectTransform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t);

        if (lifetimer < 0.0f)
        {
            Deactivate();
        }
        else
        {
            lifetimer -= Time.deltaTime;
        }
    }

    public void SetDamage(int damage)
    {
        damageText.text = damage.ToString();

        // 设置描边效果
        damageText.fontMaterial.SetColor(ShaderUtilities.ID_OutlineColor, outlineColor);
        damageText.fontMaterial.SetFloat(ShaderUtilities.ID_OutlineWidth, outlineSize); // 设置描边宽度
    }

    public void SetMiss()
    {
        damageText.text = missText;

        // 设置描边效果
        damageText.fontMaterial.SetColor(ShaderUtilities.ID_OutlineColor, outlineColor);
        damageText.fontMaterial.SetFloat(ShaderUtilities.ID_OutlineWidth, outlineSize); // 设置描边宽度
    }

    /// <summary>
    /// 继承自IPoolable接口的方法。用于对象池物体的初始化。
    /// </summary>
    public void ResetObjectState()
    {
        lifetimer = lifetime;
        rectTransform.localScale = initLocalScale;
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
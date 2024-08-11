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
    public string reloadingText = "Reloading";

    public bool crited = false;

    public Vector3 critVec = new Vector3(2.0f, 2.0f, 2.0f);

    public Color dotDamageColor = new Color(1.0f, 0.0f, 1.0f); //purple

    private Vector3 randomUpVec = new Vector3(3.0f, 3.0f, 0.0f);

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

        randomUpVec = new Vector3(Random.Range(-randomUpVec.x, randomUpVec.x), Random.Range(0.0f, randomUpVec.y), 0.0f);
    }

    private void Update()
    {
        rectTransform.position += (Vector3.up + randomUpVec).normalized * floatSpeed * Time.deltaTime;

        float t = 1.0f - (lifetimer / lifetime);
        if (crited)
        {
            rectTransform.localScale = Vector3.Lerp(critVec, Vector3.zero, t);
        }
        else
        {
            rectTransform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t);
        }

        if (lifetimer < 0.0f)
        {
            Deactivate();
        }
        else
        {
            lifetimer -= Time.deltaTime;
        }
    }

    public void SetDamage(int damage, bool isCrit = false)
    {
        damageText.text = damage.ToString();

        damageText.fontMaterial.SetFloat(ShaderUtilities.ID_OutlineWidth, outlineSize);

        if (isCrit)
        {
            damageText.color = Color.yellow;
            crited = true;
            rectTransform.localScale = critVec;
        }
        else
        {
            damageText.color = Color.white;
            damageText.fontMaterial.SetColor(ShaderUtilities.ID_OutlineColor, outlineColor);
        }
    }

    public void SetDotDamage(int damage, bool isCrit = false)
    {
        damageText.text = damage.ToString();

        damageText.fontMaterial.SetFloat(ShaderUtilities.ID_OutlineWidth, outlineSize);

        if (isCrit)
        {
            damageText.color = Color.yellow;
            crited = true;
            rectTransform.localScale = critVec;
        }
        else
        {
            damageText.color = dotDamageColor;
            damageText.fontMaterial.SetColor(ShaderUtilities.ID_OutlineColor, outlineColor);
        }
    }

    public void SetHealing(int healingAmount, bool isCrit = false)
    {
        damageText.text = healingAmount.ToString();

        damageText.fontMaterial.SetFloat(ShaderUtilities.ID_OutlineWidth, outlineSize);

        if (isCrit)
        {
            damageText.color = Color.green;
            crited = true;
            rectTransform.localScale = critVec;
        }
        else
        {
            damageText.color = Color.green;
            damageText.fontMaterial.SetColor(ShaderUtilities.ID_OutlineColor, outlineColor);
        }
    }


    public void SetMiss()
    {
        damageText.text = missText;
        damageText.color = Color.green;

        damageText.fontMaterial.SetColor(ShaderUtilities.ID_OutlineColor, outlineColor);
        damageText.fontMaterial.SetFloat(ShaderUtilities.ID_OutlineWidth, outlineSize);
    }

    public void SetReloadingClip()
    {
        damageText.text = reloadingText;
        damageText.color = Color.white;

        damageText.fontMaterial.SetColor(ShaderUtilities.ID_OutlineColor, outlineColor);
        damageText.fontMaterial.SetFloat(ShaderUtilities.ID_OutlineWidth, outlineSize);
    }

    /// <summary>
    /// 继承自IPoolable接口的方法。用于对象池物体的初始化。
    /// </summary>
    public void ResetObjectState()
    {
        lifetimer = lifetime;
        rectTransform.localScale = initLocalScale;
        crited = false;
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
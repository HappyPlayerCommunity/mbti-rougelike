using UnityEditor.ShaderKeywordFilter;
using UnityEngine;

public abstract class Surface : MonoBehaviour, IPoolable
{
    public int priority;  // 用于决定地表的覆盖优先级，数值越大优先级越高

    public float duration = -1f;
    public float durationTimer = 0.0f;

    public Vector3 initLocalScale = Vector3.one;

    private string poolKey;
    public string PoolKey
    {
        get { return poolKey; }
        set { poolKey = value; }
    }

    protected virtual void Start()
    {
        poolKey = gameObject.name;

        initLocalScale = transform.localScale;
        // 注册新生成的地表，并分配优先级
        SurfaceEffectManager.Instance?.RegisterNewSurface(this);
    }

    public virtual void ApplyEffect(GameObject obj)
    { }

    public virtual void RemoveEffect(GameObject obj)
    { }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if(TagHelper.CompareTag(other, Tag.Enemy, Tag.Player, Tag.Bond))
        {
            GameObject obj = other.gameObject;
            SurfaceEffectManager.Instance?.ApplySurfaceEffect(obj, this);
            //Debug.Log("进入地形: " + this.name);
        };
    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        if (TagHelper.CompareTag(other, Tag.Enemy, Tag.Player, Tag.Bond))
        {
            GameObject obj = other.gameObject;
            SurfaceEffectManager.Instance?.RemoveSurfaceEffect(obj, this);
            //Debug.Log("离开地形 " + this.name);
        }
    }

    protected virtual void Update()
    {
        if (durationTimer > 0.0f)
        {
            durationTimer -= Time.deltaTime;
            if (durationTimer <= 0.0f)
            {
                OnDurationEnd();
            }
        }
    }

    protected virtual void OnDurationEnd()
    {
        Deactivate();
    }

    public virtual void ReactToElement(string element, GameObject source)
    {}

    public virtual void ResetObjectState()
    {
        durationTimer = duration;
        transform.localScale = initLocalScale;
    }

    public virtual void Activate(Vector3 position, Quaternion rotation)
    {
        SurfaceEffectManager.Instance.RegisterNewSurface(this);
        transform.position = position;
        transform.rotation = rotation;
        gameObject.SetActive(true);
    }

    public virtual void Deactivate()
    {
        SurfaceEffectManager.Instance?.RemoveSurfaceEffectFromAllEntity(this);
        gameObject.SetActive(false);
        PoolManager.Instance.ReturnObject(poolKey, gameObject);
    }
}

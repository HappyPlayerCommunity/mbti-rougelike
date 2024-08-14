using UnityEngine;

public class CreamSurface : Surface
{
    public float slowAmount = 0.5f;
    public int healAmount = 1;
    public AnimationController2D healAnim;

    public float healTime = 1.0f;
    public float healTimer = 0.0f;

    public override void ApplyEffect(GameObject obj)
    {
        if (obj && obj.CompareTag(Tag.Enemy))
        {
            ApplyEnemyEffect(obj);
        }
    }

    public override void RemoveEffect(GameObject obj)
    {
        if (obj && obj.CompareTag(Tag.Enemy))
        {
            RemoveEnemyEffect(obj);
        }
    }

    private void UpdateAllyEffect(GameObject obj)
    {
        var allyUnit = obj.GetComponent<BaseEntity>();
        if (allyUnit != null)
        {
            allyUnit.GetHealing(healAmount);
            DamagePopupManager.Instance.Popup(PopupType.Healing, obj.transform.position, healAmount, false);
        
            if (healAnim)
            {
                GameObject hitEffect = PoolManager.Instance.GetObject(healAnim.name, healAnim.gameObject);
                AnimationController2D anim = hitEffect.GetComponent<AnimationController2D>();
                anim.attachedTransform = obj.transform;
                anim.Activate(obj.transform.position, Quaternion.identity);
            }
        }
    }

    protected override void Update()
    {
        if (gameObject == null) return;

        base.Update();

        healTimer -= Time.deltaTime;

        if (healTimer <= 0.0f)
        {
            foreach (var entity in SurfaceEffectManager.Instance.GetEntitiesAffectedBySurface(this))
            {
                if (entity == null)
                {
                    continue;
                }

                if (SurfaceEffectManager.Instance.IsUnderSurfaceEffect(entity, this.GetType())
                && (entity.CompareTag(Tag.Player) || entity.CompareTag(Tag.Bond)))
                {
                    UpdateAllyEffect(entity);
                }
            }

            healTimer = healTime;
        }
    }

    private void ApplyEnemyEffect(GameObject obj)
    {
        var entity = obj.GetComponent<BaseEntity>();
        if (entity != null)
        {
            entity.MovementSpeed *= slowAmount;
        }
    }

    private void RemoveEnemyEffect(GameObject obj)
    {
        var entity = obj.GetComponent<BaseEntity>();
        if (entity != null)
        {
            entity.MovementSpeed /= slowAmount;
        }
    }

}
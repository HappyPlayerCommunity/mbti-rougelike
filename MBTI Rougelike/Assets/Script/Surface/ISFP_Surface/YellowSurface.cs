using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YellowSurface : Surface
{
    public int damage = 10;
    public float damageTime = 1.0f;
    public float damageTimer = 0.0f;

    public AnimationController2D damageAnim;

    protected override void Update()
    {
        if (gameObject == null) return;

        base.Update();

        damageTimer -= Time.deltaTime;

        if (damageTimer <= 0.0f)
        {
            foreach (var entity in SurfaceEffectManager.Instance.GetEntitiesAffectedBySurface(this))
            {
                if (entity == null)
                {
                    continue;
                }

                if (SurfaceEffectManager.Instance.IsUnderSurfaceEffect(entity, this.GetType())
                && (entity.CompareTag(Tag.Enemy)))
                {
                    UpdateEnemyEffect(entity);
                }
            }

            damageTimer = damageTime;
        }
    }


    private void UpdateEnemyEffect(GameObject obj)
    {
        var entity = obj.GetComponent<BaseEntity>();
        if (entity != null)
        {
            entity.TakeDamage(damage, 0.5f);
            DamagePopupManager.Instance.Popup(PopupType.Damage, obj.transform.position, damage, false);

            if (damageAnim)
            {
                GameObject hitEffect = PoolManager.Instance.GetObject(damageAnim.name, damageAnim.gameObject);
                AnimationController2D anim = hitEffect.GetComponent<AnimationController2D>();
                anim.attachedTransform = obj.transform;
                anim.Activate(obj.transform.position, Quaternion.identity);
            }
        }
    }
}

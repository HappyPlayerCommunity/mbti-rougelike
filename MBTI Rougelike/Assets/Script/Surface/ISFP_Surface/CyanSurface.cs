using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyanSurface : Surface
{
    public int shieldRegenPreSecond = 5;

    public float shieldTime = 1.0f;
    public float shieldTimer = 0.0f;
    public AnimationController2D rearmAnim;

    protected override void Update()
    {
        if (gameObject == null) return;

        base.Update();

        shieldTimer -= Time.deltaTime;

        if (shieldTimer <= 0.0f)
        {
            foreach (var entity in SurfaceEffectManager.Instance.GetEntitiesAffectedBySurface(this))
            {
                if (entity == null)
                {
                    continue;
                }

                if (SurfaceEffectManager.Instance.IsUnderSurfaceEffect(entity, this.GetType())
                && (TagHelper.CompareTag(entity, Tag.Player, Tag.Bond)))
                {
                    UpdateAllyEffect(entity);
                }
            }

            shieldTimer = shieldTime;
        }
    }

    private void UpdateAllyEffect(GameObject obj)
    {
        var allyUnit = obj.GetComponent<BaseEntity>();
        if (allyUnit != null)
        {
            allyUnit.Shield += shieldRegenPreSecond;
            DamagePopupManager.Instance.Popup(PopupType.RearmShield, obj.transform.position, shieldRegenPreSecond, false);

            if (rearmAnim)
            {
                GameObject hitEffect = PoolManager.Instance.GetObject(rearmAnim.name, rearmAnim.gameObject);
                AnimationController2D anim = hitEffect.GetComponent<AnimationController2D>();
                anim.attachedTransform = obj.transform;
                anim.Activate(obj.transform.position, Quaternion.identity);
            }
        }
    }
}

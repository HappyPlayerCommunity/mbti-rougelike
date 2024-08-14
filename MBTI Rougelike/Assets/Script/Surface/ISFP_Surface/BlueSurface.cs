using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueSurface : Surface
{
    public int healing = 10;
    public float healingTime = 1.0f;
    public float healingTimer = 0.0f;

    public AnimationController2D healingAnim;

    protected override void Update()
    {
        if (gameObject == null) return;

        base.Update();

        healingTimer -= Time.deltaTime;

        if (healingTimer <= 0.0f)
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
                    UpdateEnemyEffect(entity);
                }
            }

            healingTimer = healingTime;
        }
    }


    private void UpdateEnemyEffect(GameObject obj)
    {
        var entity = obj.GetComponent<BaseEntity>();
        if (entity != null)
        {
            entity.GetHealing(healing);
            DamagePopupManager.Instance.Popup(PopupType.Healing, obj.transform.position, healing, false);

            if (healingAnim)
            {
                GameObject hitEffect = PoolManager.Instance.GetObject(healingAnim.name, healingAnim.gameObject);
                AnimationController2D anim = hitEffect.GetComponent<AnimationController2D>();
                anim.attachedTransform = obj.transform;
                anim.Activate(obj.transform.position, Quaternion.identity);
            }
        }
    }
}

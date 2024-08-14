using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurpleSurface : Surface
{
    public Status poisonStatus;
    public float poisonTime = 1.0f;
    public float poisonTimer = 0.0f;

    public AnimationController2D poisonAnim;

    Stats stats;

    protected override void Start()
    {
        base.Start();
        stats = FindObjectOfType<Player>().stats;
    }

    protected override void Update()
    {
        if (gameObject == null) return;

        base.Update();

        poisonTimer -= Time.deltaTime;

        if (poisonTimer <= 0.0f)
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

            poisonTimer = poisonTime;
        }
    }


    private void UpdateEnemyEffect(GameObject obj)
    {
        var entity = obj.GetComponent<BaseEntity>();
        if (entity != null)
        {
            entity.StatusManager?.AddStatus(poisonStatus, stats);

            if (poisonAnim)
            {
                GameObject hitEffect = PoolManager.Instance.GetObject(poisonAnim.name, poisonAnim.gameObject);
                AnimationController2D anim = hitEffect.GetComponent<AnimationController2D>();
                anim.attachedTransform = obj.transform;
                anim.Activate(obj.transform.position, Quaternion.identity);
            }
        }
    }
}

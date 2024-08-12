using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DamageCollider;

/// <summary>
/// 一种【伤害块】变体，用来处理可以同时击中不同tag并执行不同效果的伤害块逻辑。
/// </summary>
public class MultiEffectDamageCollider : DamageCollider
{
    [SerializeField, Tooltip("会产生【治疗效果】的其他物体的tag。")]
    protected List<string> healTags;

    protected override void DamageToObject(BaseEntity entity, Collider2D hit)
    {
        if (healTags.Contains(hit.tag))
        {
            isHealingMode = true;
            ExecuteHealingLogic(entity, hit);
        }
        else if (effectTags.Contains(hit.tag))
        {
            isHealingMode = false;
            ExecuteDamageLogic(entity, hit);
        }
    }

    private void ExecuteHealingLogic(BaseEntity entity, Collider2D hit)
    {
        // 注册此治疗行为
        DamageManager.RegisterDamage(gameObject, entity);

        DamagePopupManager.Instance.Popup(PopupType.Healing, hit.transform.position, damage, false);
        entity.GetHealing(damage);
        entity.SetDamageTimer(gameObject, damageTriggerTime);
        HitAnimation(hit.transform);

        didDamage = true;

        //if (applyStatus && entity.StatusManager && entity.IsAlive())
        //{
        //    if (owner is Player)
        //    {
        //        var player = (Player)owner;
        //        entity.StatusManager.AddStatus(applyStatus, player.stats);
        //    }
        //    else
        //    {
        //        entity.StatusManager.AddStatus(applyStatus, null);
        //    }
        //}
    }

    private void ExecuteDamageLogic(BaseEntity entity, Collider2D hit)
    {
        // 注册此伤害行为
        DamageManager.RegisterDamage(gameObject, entity);

        if (entity is Unit)
        {
            var unit = (Unit)entity;

            // 如果【碰撞体】是Unit类型，则结算【闪避】和【吹飞】效果。
            if (TryHit(unit))
            {
                BlowUpEntity(unit);
            }
            else
            {
                // 使【伤害块】短时间无法再对该目标造成伤害。
                entity.SetDamageTimer(gameObject, damageTriggerTime);
                DamagePopupManager.Instance.Popup(PopupType.Miss, hit.transform.position);
                return;
            }
        }

        // 暴击默认为false，如果暴击，则会在CalculateDamage中修改。
        bool isCrit = false;
        int finalDamage = DamageManager.CalculateDamage(damageType, damage, owner, ref isCrit, damageElementType);

        // 对实体造成伤害并设置击晕时间
        entity.TakeDamage(finalDamage, staggerTime);

        DamagePopupManager.Instance.Popup(PopupType.Damage, hit.transform.position, finalDamage, isCrit);

        // 令该实体保存一个对此【伤害块】的计时器，短时间无法再对其造成伤害。
        entity.SetDamageTimer(gameObject, damageTriggerTime);

        if (owner is Player)
        {
            var player = (Player)owner;
            float boostCharge = player.stats.Calculate_AttackEnergeCharge();
            player.personality.AttackChargeEnerge(finalDamage, boostCharge); // 受伤充能比率还得具体设计。
        }

        switch (hitEffectPlayMode)
        {
            case HitEffectPlayMode.HitPoint: // hmm，不太完善。
                RaycastHit2D raycastHit = Physics2D.Raycast(transform.position, (hit.transform.position - transform.position).normalized);
                if (raycastHit.collider != null)
                {
                    Transform collisionPoint = transform;
                    collisionPoint.position = raycastHit.point;
                    HitAnimation(collisionPoint);
                }
                break;
            case HitEffectPlayMode.Target:
                HitAnimation(hit.transform);
                break;
            default:
                break;
        }

        didDamage = true;

        if (applyStatus && entity.StatusManager && entity.IsAlive())
        {
            if (owner is Player)
            {
                var player = (Player)owner;
                entity.StatusManager.AddStatus(applyStatus, player.stats);
            }
            else
            {
                entity.StatusManager.AddStatus(applyStatus, null);
            }
        }
    }
}

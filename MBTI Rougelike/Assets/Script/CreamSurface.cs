using System.Collections.Generic;
using UnityEngine;

public class CreamSurface : Surface
{
    public float slowAmount = 0.5f;
    public int healAmount = 1;
    public AnimationController2D healAnim;

    public float healTime = 1.0f;
    public float healTimer = 0.0f;

    protected override void InitializeTagEffectMap()
    {
        base.InitializeTagEffectMap();
        tagEffectMap[Tag.Enemy] = ApplySlowEffect;
    }

    protected override void InitializeRemoveEffectMap()
    {
        base.InitializeRemoveEffectMap();
        removeEffectMap[Tag.Enemy] = RemoveSlowEffect;
    }

    protected override void InitializeUpdateEffectMap()
    {
        base.InitializeUpdateEffectMap();
        updateEffectMap[Tag.Player] = UpdateHealingEffect;
        updateEffectMap[Tag.Bond] = UpdateHealingEffect;
    }

    private void ApplySlowEffect(GameObject unit)
    {
        var movementController = unit.GetComponent<BaseEntity>();
        if (movementController != null)
        {
            movementController.MovementSpeed *= slowAmount;
        }
    }

    private void RemoveSlowEffect(GameObject unit)
    {
        var movementController = unit.GetComponent<BaseEntity>();
        if (movementController != null)
        {
            movementController.MovementSpeed /= slowAmount;
        }
    }

    private void UpdateHealingEffect(GameObject unit)
    {
        healTimer -= Time.deltaTime;
        if (healTimer < 0.0f)
        {
            var allyUnit = unit.GetComponent<Unit>();
            if (allyUnit != null)
            {
                allyUnit.GetHealing(healAmount);
                DamagePopupManager.Instance.Popup(PopupType.Healing, unit.transform.position, healAmount, false);

                if (healAnim)
                {
                    GameObject hitEffect = PoolManager.Instance.GetObject(healAnim.name, healAnim.gameObject);
                    AnimationController2D anim = hitEffect.GetComponent<AnimationController2D>();
                    anim.attachedTransform = unit.transform;
                    anim.Activate(unit.transform.position, Quaternion.identity);
                }
            }

            healTimer = healTime;
        }
    }
}

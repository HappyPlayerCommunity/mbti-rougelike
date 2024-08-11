using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPoisonStatus", menuName = "Status Data/Poison Status")]
public class PoisonStatus : Status
{
    public int poisonDamage = 10;
    public float positionTime = 1.0f;
    public float positionTimer = 0.0f;

    public override void OnUpdate(GameObject target, float deltaTime)
    {
        //临时标识，后续添加特效。
        target.GetComponentInChildren<SpriteRenderer>().color = new Color(1.0f, 0.0f, 1.0f);

        if (positionTimer > 0.0f)
        {
            positionTimer -= deltaTime;
        }
        else
        {
            positionTimer = positionTime * (1.0f / modifyPowerRate);
            var unit = target.GetComponent<Unit>();
            if (unit)
            {
                int finalDamage = (int)(poisonDamage * modifyImpactRate);
                DamagePopupManager.Instance.Popup(PopupType.DotDamage, unit.transform.position, finalDamage);
                unit.TakeDamage(finalDamage, deltaTime);

                PlayAnimation(target);
            }
        }

        base.OnUpdate(target, deltaTime);
    }

    public override void OnApply(GameObject target)
    {
        positionTimer = positionTime;
        base.OnApply(target);

    }

    public override void OnStack(Status status)
    {
        base.OnStack(status);
        PoisonStatus poisonStatus = (PoisonStatus)status;
        poisonDamage += poisonStatus.poisonDamage;
        positionTime = Mathf.Min(positionTime, poisonStatus.positionTime);
    }

    public override void OnExpire(GameObject target)
    {
        base.OnExpire(target);
    }
}

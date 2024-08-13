using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EstpSpeedBoost : DamageColliderBoost
{
    private EstpController estpController;


    public override void Boost(DamageCollider dc)
    {
        estpController = dc.owner.transform.GetComponent<EstpController>();

        if (estpController != null)
        {
            float speed = estpController.currentSpeed;
            float maxSpeed = estpController.maxSpeed;
            float rate = speed / maxSpeed;

            float easedRate = Mathf.Pow(rate, 2);

            dc.damage = CalculateDamageBasedOnSpeed(dc.damage, easedRate);
            dc.BlowForceSpeed = CalculateBlowforceBasedOnSpeed(dc.BlowForceSpeed, easedRate);
        }
    }

    private int CalculateDamageBasedOnSpeed(int damage, float rate)
    {
        rate = Mathf.Max(rate, 0.1f); // 速度为 0 时，伤害至少为 10%
        return (int)(damage * rate);
    }

    private float CalculateBlowforceBasedOnSpeed(float blowForceSpeed, float rate)
    {
        return blowForceSpeed * rate;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using static Skill;

public class AttackHelper : MonoBehaviour
{

    public static void InitSkillDamageCollider(Skill skill, Transform initPos, float chargingRate, Player player, float adjustBackOffset, Vector3 aimDirection, float scatterAngle)
    {
        InitDamageCollider(skill.DamageCollider, initPos, adjustBackOffset, aimDirection, scatterAngle, skill.ControlScheme, skill.FixPos, chargingRate, skill.GetRenderMode, player, skill.DamageColliderSpeed);
    }

    public static void InitTurretDamageCollider(DamageCollider damageColliderRef, Transform initPos, Vector3 aimDirection, float scatterAngle, bool isFixPos, Skill.RenderMode renderMode, Player player, float damageColliderSpeed)
    {
        InitDamageCollider(damageColliderRef, initPos, 0.0f, aimDirection, scatterAngle, SkillControlScheme.None, isFixPos, 1.0f, renderMode, player, damageColliderSpeed);
    }

    public static void InitDamageCollider(DamageCollider damageColliderRef, Transform initPos, float adjustBackOffset, Vector3 aimDirection, float scatterAngle, SkillControlScheme controlScheme, bool isFixPos, float chargingRate, Skill.RenderMode renderMode, Player player, float damageColliderSpeed)
    {
        string poolKey = damageColliderRef.name;
        GameObject damageColliderObj = PoolManager.Instance.GetObject(poolKey, damageColliderRef.gameObject);
        DamageCollider damageCollider = damageColliderObj.GetComponent<DamageCollider>();

        var sprite = damageCollider.GetComponentInChildren<SpriteRenderer>();
        var collider = damageCollider.GetComponentInChildren<Collider2D>();

        // 计算生成位置
        Vector3 spawnPosition = initPos.position;

        if (controlScheme == SkillControlScheme.ChargeRelease)
        {
            SkillChargingRateUpdate(damageCollider, chargingRate);
        }

        damageCollider.owner = player;

        if (isFixPos)
        {
            switch (collider)
            {
                case BoxCollider2D boxCollider:
                    float boxWidth = boxCollider.size.x * sprite.transform.localScale.x;
                    float boxOffset = boxCollider.offset.x * sprite.transform.localScale.x;

                    float distanceFromPlayer = boxWidth * 0.5f + boxOffset;
                    float finalAdjustBack = adjustBackOffset * chargingRate;

                    spawnPosition = player.transform.position + aimDirection.normalized * (distanceFromPlayer - finalAdjustBack);
                    break;
                default:
                    // Todo: 其他类型的collider
                    break;
            }
        }

        damageCollider.InitPos = aimDirection.normalized * (initPos.position - damageCollider.owner.transform.position).magnitude;

        damageCollider.Activate(spawnPosition, Quaternion.Euler(0.0f, 0.0f, 0.0f));

        if (sprite)
        {
            float angle = Vector2.SignedAngle(new Vector2(1.0f, 0.0f), aimDirection);
            sprite.transform.localEulerAngles = new Vector3(0.0f, 0.0f, angle);

            switch (renderMode)
            {
                case Skill.RenderMode.HorizontalFlip:
                    if (aimDirection.x < 0.0f)
                    {
                        sprite.transform.localScale = new Vector3(sprite.transform.localScale.x, -sprite.transform.localScale.y, sprite.transform.localScale.z);
                    }
                    break;
                case Skill.RenderMode.AllFlip:
                    if (aimDirection.x < 0.0f)
                    {
                        sprite.transform.localScale = new Vector3(-sprite.transform.localScale.x, -sprite.transform.localScale.y, sprite.transform.localScale.z);
                    }
                    break;
                case Skill.RenderMode.Lock:
                    sprite.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                    break;
                default:
                    break;
            }

            if (collider != null)
            {
                collider.transform.localEulerAngles = sprite.transform.localEulerAngles;
            }
            // 以前的实现方法，备存一下。

            //// 如果瞄准方向是向左的，则需要将角度加180度，因为Sprite【默认面向右侧】
            //if (aimDirection.x < 0.0f)
            //{
            //    angle += 180.0f;
            //}

            //// 直接根据y轴方向调整旋转角度
            //sprite.transform.localEulerAngles = aimDirection.y > 0.0f ? new Vector3(0.0f, 0.0f, angle) : new Vector3(0.0f, 0.0f, -angle);

        }

        float scatterAngleHalf = scatterAngle / 2.0f;

        float randomAngle = Random.Range(-scatterAngleHalf, scatterAngleHalf);

        Vector3 scatterDirection = Quaternion.Euler(0, 0, randomAngle) * aimDirection;

        Vector3 finalVelocity = scatterDirection.normalized * damageColliderSpeed;
        damageCollider.Velocity = finalVelocity;
    }

    private static void SkillChargingRateUpdate(DamageCollider damageCollider, float chargingRate)
    {
        damageCollider.damage += (int)(damageCollider.ChargingDamage * chargingRate);

        damageCollider.BlowForceSpeed += damageCollider.ChargingBlowForceSpeed * chargingRate;

        //damageCollider.Velocity = damageCollider.Velocity * chargingRate;

        //damageCollider.MaxTimer = damageCollider.MaxTimer * chargingRate;
        //damageCollider.Timer = damageCollider.MaxTimer;

        damageCollider.StaggerTime += damageCollider.ChargingStaggerTime * chargingRate;

        switch (damageCollider.damageMovementType)
        {
            case DamageCollider.DamageMovementType.Passive:
                damageCollider.spriteRenderer.transform.localScale += damageCollider.ChargingLocalScale * chargingRate;
                break;
            case DamageCollider.DamageMovementType.Projectile:
                damageCollider.MaxTimer += damageCollider.ChargingMaxTimer * chargingRate;
                damageCollider.Timer = damageCollider.MaxTimer;
                break;
            default:
                break;
        }
    }
}

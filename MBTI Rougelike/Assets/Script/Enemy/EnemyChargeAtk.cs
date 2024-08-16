using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChargeAtk : EnemySpecialImpl
{
    public float chargeSpeed = 10.0f;

    public override void ExecuteSpecialImplementation(Enemy enemy)
    {
        var player = enemy.Player;

        var distanceVec = player.transform.position - enemy.transform.position;
        Vector3 direction = Vector3.Normalize(distanceVec);

        float distance = Vector3.Distance(player.transform.position, enemy.transform.position);

        enemy.Velocity = Vector3.zero;

        if (distance <= enemy.AttackRange)
        {
            if (enemy.AttackTimer <= 0.0f)
            {
                string poolKey = enemy.DamageCollider.name;
                GameObject damageColliderObj = PoolManager.Instance.GetObject(poolKey, enemy.DamageCollider.gameObject);
                DamageCollider collider = damageColliderObj.GetComponent<DamageCollider>();
                collider.Activate(enemy.transform.position + (distanceVec.normalized * enemy.DamageColliderInitDistance), Quaternion.Euler(0.0f, 0.0f, 0.0f));
                collider.owner = enemy.transform.GetComponent<Unit>();
                collider.Velocity = (distanceVec.normalized * enemy.DamageColliderMovementSpeed);

                var sprite = collider.GetComponentInChildren<SpriteRenderer>();
                if (sprite != null)
                {
                    float angle = Vector2.SignedAngle(new Vector2(1.0f, 0.0f), direction);

                    switch (enemy.DamageColliderRenderMode)
                    {
                        case Skill.RenderMode.HorizontalFlip:
                            sprite.transform.localEulerAngles = new Vector3(0.0f, 0.0f, angle);

                            if (direction.x < 0.0f)
                            {
                                sprite.transform.localScale = new Vector3(sprite.transform.localScale.x, -sprite.transform.localScale.y, sprite.transform.localScale.z);
                            }
                            break;
                        case Skill.RenderMode.AllFlip:
                            sprite.transform.localEulerAngles = new Vector3(0.0f, 0.0f, angle);

                            if (direction.x < 0.0f)
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

                    var collider2d = enemy.DamageCollider.GetComponentInChildren<Collider2D>();
                    if (collider2d != null)
                    {
                        collider2d.transform.localEulerAngles = sprite.transform.localEulerAngles;
                    }
                }

                enemy.BlowForceVelocity = direction * chargeSpeed;

                enemy.AttackTimer = enemy.AttackTime;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerTrigger : MonoBehaviour
{
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Tag.ISTP_Robot))
        {
            var robot = other.GetComponentInParent<IstpRobot>();
            if (robot != null)
            {
                switch (robot.currentMode)  
                {
                    case RobotMode.Melee:
                        robot.currentMode = RobotMode.Ranged;
                        robot.SpriteRenderer.sprite = robot.rangedSprite;
                        break;
                    case RobotMode.Ranged:
                        robot.currentMode = RobotMode.Melee;
                        robot.SpriteRenderer.sprite = robot.meleeSprite;
                        break;
                    default:
                        break;
                }

                var damageCollier = transform.GetComponentInParent<DamageCollider>();
                damageCollier.Velocity = -damageCollier.Velocity;
            }
        }

    }
}

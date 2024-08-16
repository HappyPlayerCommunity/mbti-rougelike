using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChargeIdle : EnemySpecialImpl
{
    public float idleSpeed = 2.0f;  // 控制敌人在Idle状态下的移动速度
    public float directionChangeInterval = 3.0f;  // 控制敌人保持相同方向的时间间隔

    public float detectCollisionRadius = 0.1f;  // 用于检测敌人是否撞到物体的半径
    public Vector3 currentDirection;
    private float timer;

    public LayerMask collisionLayer;

    public override void ExecuteSpecialImplementation(Enemy enemy)
    {
        // 如果计时器已经到达方向改变的时间间隔，或者敌人撞到了物体，则重新生成方向
        if (timer <= 0.0f || IsColliding(enemy))
        {
            // 随机生成一个新的方向
            currentDirection = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 0.0f).normalized;
            timer = directionChangeInterval;  // 重置计时器
        }

        // 使敌人沿着当前方向移动
        enemy.Velocity = currentDirection * idleSpeed;

        // 更新计时器
        timer -= Time.deltaTime;
    }

    // 检测敌人是否撞到了任何物体
    private bool IsColliding(Enemy enemy)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(enemy.transform.position, currentDirection, detectCollisionRadius);

        foreach (var hit in hits)
        {
            // 检查检测到的碰撞体是否是敌人自身或其子对象
            if (hit.collider != null && hit.collider.gameObject != enemy.gameObject)
            {
                return true;  // 检测到碰撞，且不是敌人自身
            }
        }

        return false;  // 没有检测到非敌人的碰撞
    }
}
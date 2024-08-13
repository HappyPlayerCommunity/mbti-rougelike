using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngelTurret : Turret
{
    public float followRadius = 3.0f; // 跟随半径
    public float moveSpeed = 1.0f; // 移动速度
    public float activationRadius = 5.0f; // 开始追随的半径
    public float stopBuffer = 0.5f; // 停止缓冲区

    public float floatAmplitude = 0.5f; // 浮动幅度
    public float floatFrequency = 1.0f; // 浮动频率
    private Vector3 initialPosition;

    private Vector3 targetPosition;

    protected override void Start()
    {
        base.Start();
        initialPosition = transform.position; // 记录初始位置
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        FollowPlayer();
        FloatMovement();

        // hmm，可能应该写在fixupdate里。
    }

    private void FollowPlayer()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer > activationRadius)
        {
            // 追随玩家到指定半径
            Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
            targetPosition = player.transform.position - directionToPlayer * followRadius;
            transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
        else if (distanceToPlayer > followRadius + stopBuffer)
        {
            // 玩家离开检测距离，追随玩家
            Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
            targetPosition = player.transform.position - directionToPlayer * followRadius;
            transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
        else if (distanceToPlayer < followRadius - stopBuffer)
        {
            // 玩家太近，稍微远离玩家
            Vector3 directionToPlayer = (transform.position - player.transform.position).normalized;
            targetPosition = player.transform.position + directionToPlayer * followRadius;
            transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
        // 否则，保持当前位置
    }

    private void FloatMovement()
    {
        float newY = initialPosition.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}

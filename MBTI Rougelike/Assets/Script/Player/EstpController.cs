using System.Collections;
using System.Collections.Generic;
using UnityEditor.Networking.PlayerConnection;
using UnityEngine;

public class EstpController : PlayerController
{
    public float acceleration = 2.0f; // 滑板加速度
    public float maxSpeed = 10.0f; // 滑板最大速度
    public float turnDeceleration = 5.0f; // 拐弯减速
    public float decelerationRate = 0.95f; // 滑行减速率
    public float turnSmoothTime = 0.2f; // 转向平滑时间
    public float collisionSpeedReductionFactor = 0.2f; // 碰撞时速度减少的比例

    public float currentSpeed = 0.0f; // 当前速度
    public Vector3 currentDirection = Vector3.zero; // 当前移动方向
    public Vector3 targetDirection = Vector3.zero; // 目标移动方向
    public float turnSmoothVelocity; // 转向平滑速度

    public bool ultRocket = false;

    public Rigidbody2D rb;
    public Collider2D col;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (statusManager.IsRooted())
        {
            player.Velocity = Vector3.zero;
            return;
        }

        HandleMovement();
    }

    void HandleMovement()
    {
        Vector3 direction = Vector3.zero;

        // 获取输入方向
        if (Input.GetKey(KeyCode.W))
        {
            direction += Vector3.up;
        }
        if (Input.GetKey(KeyCode.S))
        {
            direction += Vector3.down;
        }
        if (Input.GetKey(KeyCode.A))
        {
            direction += Vector3.left;
        }
        if (Input.GetKey(KeyCode.D))
        {
            direction += Vector3.right;
        }

        direction = direction.normalized;

        if (direction != Vector3.zero)
        {
            // 如果方向改变，减速
            if (direction != targetDirection)
            {
                currentSpeed -= turnDeceleration * Time.deltaTime;
                currentSpeed = Mathf.Max(currentSpeed, 0.0f); // 确保速度不为负
            }

            // 更新目标方向
            targetDirection = direction;

            // 加速
            currentSpeed += acceleration * Time.deltaTime * player.stats.Calculate_MovementSpeed();
            currentSpeed = Mathf.Min(currentSpeed, maxSpeed); // 确保速度不超过最大值
        }
        else
        {
            // 如果没有输入方向，逐渐减速
            currentSpeed *= decelerationRate;
        }

        currentSpeed *= player.StaggerRate;

        // 平滑转向
        currentDirection = Vector3.Slerp(currentDirection, targetDirection, turnSmoothTime * Time.deltaTime);

        // 移动
        transform.position += currentDirection * currentSpeed * Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 当撞墙时，大幅度减少当前速度
        if (!ultRocket)
        {
            currentSpeed *= collisionSpeedReductionFactor;
        }
    }
}

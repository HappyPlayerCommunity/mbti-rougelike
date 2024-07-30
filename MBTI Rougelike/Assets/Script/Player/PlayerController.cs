using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 用来控制角色移动的类。它将管控角色的各种输入，并转换成移动速度等影响玩家实体。
/// </summary>
public class PlayerController : MonoBehaviour
{
    public Player player;
    public StatusManager statusManager;

    [SerializeField, Tooltip("当前玩家的速度向量值。")]
    private Vector3 velocity;

    //[SerializeField, Tooltip("玩家的移动速度，后面改为由PlayerData中读取。")]
    //private float movementSpeed = 5.0f;

    [SerializeField, Tooltip("玩家攻击时的移动速度，后面改为由PlayerData中读取。")]
    private float movementSpeedReduceRate = 0.6f;

    void Start()
    {
        player = GetComponent<Player>();
        statusManager = GetComponent<StatusManager>();
    }

    void Update()
    {
        if (statusManager.IsRooted())
        {
            player.Velocity = Vector3.zero;
            return;
        }

        Vector3 direction = new Vector3(0.0f, 0.0f, 0.0f);

        float finalMovementSpeed = player.IsActioning ? 
            player.stats.Calculate_MovementSpeed() * movementSpeedReduceRate : player.stats.Calculate_MovementSpeed();

        // 后续将绑定到专门的按键类，为了改键。
        if (Input.GetKey(KeyCode.W))
            direction.y = 1.0f;

        if (Input.GetKey(KeyCode.S))
            direction.y = -1.0f;

        if (Input.GetKey(KeyCode.A))
            direction.x = -1.0f;

        if (Input.GetKey(KeyCode.D))
            direction.x = 1.0f;

        if (direction != Vector3.zero)
            direction.Normalize();

        velocity = direction * finalMovementSpeed;
        player.Velocity = velocity;
    }
}
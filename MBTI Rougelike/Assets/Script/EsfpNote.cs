using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EsfpNote : MonoBehaviour
{
    [Tooltip("旋转速度")]
    public float rotationSpeed = 50.0f;

    [Tooltip("环绕半径")]
    public float radius = 2.0f;

    private Transform playerTransform;
    private float angle;

    // Start is called before the first frame update
    void Start()
    {
        // 找到玩家的 Transform
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        if (playerTransform == null)
        {
            Debug.LogError("Player not found. Make sure the player has the 'Player' tag.");
        }

        // 初始化角度
        angle = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTransform == null) return;

        // 更新角度
        angle += rotationSpeed * Time.deltaTime;

        // 计算音符的位置
        float radian = angle * Mathf.Deg2Rad;
        Vector3 position = new Vector3(
            playerTransform.position.x + radius * Mathf.Cos(radian),
            playerTransform.position.y + radius * Mathf.Sin(radian),
            playerTransform.position.z
        );

        // 设置音符的位置
        transform.position = position;
    }
}

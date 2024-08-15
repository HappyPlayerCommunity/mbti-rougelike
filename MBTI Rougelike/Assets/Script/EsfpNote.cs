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
    public Aim aim;
    private float angle;

    public EsfpNoteManager esfpNoteManager;

    public bool isBigNote = false;

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
        if (playerTransform == null || aim == null) return;

        // 更新角度
        angle += rotationSpeed * Time.deltaTime;

        // 获取 Aim 的角度
        float aimAngle = aim.GetAimAngle(); // 假设 Aim 类有一个 GetAimAngle 方法返回当前瞄准角度

        // 计算音符的位置
        float totalAngle = angle + aimAngle + 180;
        float radian = totalAngle * Mathf.Deg2Rad;
        Vector3 position = new Vector3(
            playerTransform.position.x + radius * Mathf.Cos(radian),
            playerTransform.position.y + radius * Mathf.Sin(radian),
            playerTransform.position.z
        );

        // 设置音符的位置
        transform.position = position;
    }
}

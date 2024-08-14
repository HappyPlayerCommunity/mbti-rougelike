using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Palette : PersonalitySpecialImplementation
{
    [SerializeField, Tooltip("UltraSun的Prefab")]
    private List<Surface> surfaces;

    [Tooltip("环形生成的半径")]
    public float radius = 5.0f;

    [Tooltip("颜料扩大率")]
    public float expandRate = 3.0f;

    public override void ExecuteSpecialImplementation(Personality personality)
    {
        if (surfaces == null || surfaces.Count < 8)
        {
            Debug.LogError("There must be at least 8 surfaces in the list.");
            return;
        }

        System.Random random = new System.Random();
        surfaces = surfaces.OrderBy(x => random.Next()).ToList();

        // 获取玩家的位置
        Vector3 playerPosition = personality.transform.position;

        // 计算每个 Surface 的角度间隔
        float angleStep = 360.0f / 8;

        for (int i = 0; i < 8; i++)
        {
            // 计算当前 Surface 的角度
            float angle = i * angleStep;
            float radian = angle * Mathf.Deg2Rad;

            // 计算 Surface 的位置
            Vector3 position = new Vector3(
                playerPosition.x + radius * Mathf.Cos(radian),
                playerPosition.y + radius * Mathf.Sin(radian), // 在2D平面上，使用y轴
                playerPosition.z
            );

            // 实例化 Surface
            GameObject newSurfaceObj = PoolManager.Instance.GetObject(surfaces[i].name, surfaces[i].gameObject);
            Surface newSurface = newSurfaceObj.GetComponent<Surface>();

            newSurface.transform.localScale *= expandRate;
            newSurface.Activate(position, Quaternion.identity);
        }
    }
}

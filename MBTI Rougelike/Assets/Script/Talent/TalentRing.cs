using UnityEngine;

public class TalentRing : MonoBehaviour
{
    public TalentSlot[] talentSlots; // 天赋槽的数组（UI对象）
    public float screenRadiusPercentage = 0.3f; // 用屏幕的百分比来计算半径
    public Stats stats;

    private EdgeCollider2D edgeCollider;
    private float ringRadius;

    void Start()
    {
        ArrangeTalentSlotsInRing();
        SetupEdgeCollider();
    }

    // 用于环形排列天赋槽的方法
    void ArrangeTalentSlotsInRing()
    {
        float screenShortSide = Mathf.Min(Screen.width, Screen.height);
        ringRadius = screenShortSide * screenRadiusPercentage;

        int numSlots = talentSlots.Length;
        for (int i = 0; i < numSlots; i++)
        {
            // 计算每个槽位的角度
            float angle = i * Mathf.PI * 2 / numSlots;
            // 根据角度计算槽位的位置
            Vector3 position = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * ringRadius;
            // 设置槽位的 UI 元素位置
            talentSlots[i].GetComponent<RectTransform>().anchoredPosition = position;

            talentSlots[i].playerStats = stats;
        }
    }

    // 设置边界碰撞器
    void SetupEdgeCollider()
    {
        edgeCollider = gameObject.AddComponent<EdgeCollider2D>();
        int pointCount = 100; // 边界点的数量
        Vector2[] points = new Vector2[pointCount + 1];

        for (int i = 0; i <= pointCount; i++)
        {
            float angle = i * 2 * Mathf.PI / pointCount;
            points[i] = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * ringRadius;
        }
        edgeCollider.points = points;
    }

    public EdgeCollider2D GetEdgeCollider()
    {
        return edgeCollider;
    }

    public float GetRingRadius()
    {
        return ringRadius;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalentRing : MonoBehaviour
{
    public TalentSlot[] talentSlots; // 天赋槽的数组（UI对象）
    public float ringRadius = 200f;

    void Start()
    {
        ArrangeTalentSlotsInRing();
    }

    // 用于环形排列天赋槽的方法
    void ArrangeTalentSlotsInRing()
    {
        int numSlots = talentSlots.Length;
        for (int i = 0; i < numSlots; i++)
        {
            // 计算每个槽位的角度
            float angle = i * Mathf.PI * 2 / numSlots;
            // 根据角度计算槽位的位置
            Vector3 position = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * ringRadius;
            // 设置槽位的 UI 元素位置
            talentSlots[i].GetComponent<RectTransform>().anchoredPosition = position;
        }
    }
}

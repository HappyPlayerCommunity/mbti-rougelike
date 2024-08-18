using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TalentSlot : MonoBehaviour
{
    public Button slotButton;
    public Sprite emptySlotSprite; // 空槽的图片
    public Sprite filledSlotSprite; // 填充槽的图片

    private Talent currentTalent;

    // 初始化槽位为空状态
    public void InitializeEmptySlot()
    {
        slotButton.image.sprite = emptySlotSprite;
        currentTalent = null;
    }

    // 当放入天赋时更新槽位状态
    public void FillSlotWithTalent(Talent talent)
    {
        currentTalent = talent;
        slotButton.image.sprite = filledSlotSprite; // 使用天赋图片或专属图片
    }

    // 用于检测玩家点击槽位的事件
    public void OnSlotClick()
    {
        if (currentTalent != null)
        {
            // 显示天赋详情或者处理点击逻辑
        }
    }
}
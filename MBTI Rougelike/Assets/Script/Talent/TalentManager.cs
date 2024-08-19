using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TalentManager : MonoBehaviour
{
    public Talent[] equippedTalents; // 当前装备的天赋，大小为10个
    public int maxSlots = 10; // 天赋槽数量
    public Stats stats;

    public List<Talent> talentsPool;
    public TalentReward talentRewardPrefab;
    public Transform ringCenter; // 环的中心位置
    public float floatingRadius = 50f; // 漂浮半径
    public float floatingSpeed = 1f; // 漂浮速度

    public int maxRewardCount = 3; // 最大奖励数量
    public Canvas canvas; // Canvas 对象

    public void Start()
    {
        stats.Initialize();
    }

    public void EquipTalent(Talent newTalent, int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= maxSlots)
            return;

        if (equippedTalents[slotIndex] == null)
        {
            equippedTalents[slotIndex] = newTalent;
        }
        else
        {
            // 处理融合逻辑
            FuseTalents(equippedTalents[slotIndex], newTalent);
        }
    }

    private void FuseTalents(Talent existingTalent, Talent newTalent)
    {
        // 你可以在这里调用TalentSlot类的FuseTalents方法来进行融合逻辑
        // 比如：调用现有槽位的FuseTalents方法
    }

    public void ApplyTalentEffects()
    {
        // 遍历所有装备的天赋并应用它们的效果
        foreach (var talent in equippedTalents)
        {
            if (talent != null)
            {
                talent.ApplyTalent(stats);
            }
        }
    }

    public void GenerateTalentRewards()
    {
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        float canvasHeight = canvasRect.rect.height;

        float spaceY = 150.0f;

        float totalHeight = (maxRewardCount - 1) * spaceY;
        float startY = totalHeight / 2;
        float x = 0;

        for (int i = 0; i < maxRewardCount; i++)
        {
            TalentReward talentReward = GenerateRandomTalentReward();
            talentReward.transform.SetParent(canvas.transform, false);

            float y = startY - i * spaceY;

            RectTransform rewardRectTransform = talentReward.GetComponent<RectTransform>();
            rewardRectTransform.anchoredPosition = new Vector2(x, y);
        }
    }


    public TalentReward GenerateRandomTalentReward()
    {
        // 随机从talentsPool中选择一个天赋
        int randomIndex = Random.Range(0, talentsPool.Count);
        Talent randomTalent = talentsPool[randomIndex];

        // 实例化一个天赋奖励对象
        TalentReward talentReward = Instantiate(talentRewardPrefab);
        talentReward.talent = randomTalent;
        talentReward.textMesh.text = randomTalent.description;

        //talentReward.GetComponent<Image>().color = randomTalent.color;
        //talentReward.
        //var texts = talentReward.GetComponentsInChildren<TextMeshProUGUI>();
        //texts[0].text = randomTalent.type.ToString(); // 显示天赋类型
        //texts[1].text = randomTalent.description; // 显示天赋描述

        return talentReward;
    }
}

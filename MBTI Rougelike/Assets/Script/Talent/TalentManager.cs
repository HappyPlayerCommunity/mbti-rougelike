using UnityEngine;

public class TalentManager : MonoBehaviour
{
    public Talent[] equippedTalents; // 当前装备的天赋，大小为10个
    public int maxSlots = 10; // 天赋槽数量

    public Stats stats;

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
        // 实现融合逻辑，例如升级为进阶或高阶天赋
        // 可以随机决定是否进化为更高等级天赋
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
}

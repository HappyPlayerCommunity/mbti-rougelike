using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TalentSlot : MonoBehaviour
{
    public Button slotButton;
    public Sprite emptySlotSprite; // 空槽的图片
    public Sprite filledSlotSprite; // 填充槽的图片
    public Stats playerStats; // 玩家Stats的引用

    public Talent currentTalent;

    // 初始化槽位为空状态
    public void InitializeEmptySlot()
    {
        slotButton.image.sprite = emptySlotSprite;
        currentTalent = null;
    }

    // 当放入天赋时更新槽位状态并应用天赋效果
    public void FillSlotWithTalent(Talent talent)
    {
        if (currentTalent == null)
        {
            currentTalent = talent;
            //slotButton.image.sprite = filledSlotSprite; // 使用天赋图片或专属图片
            slotButton.image.color = talent.color; // 使用天赋颜色
            slotButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = talent.type.ToString(); // 显示天赋类型
            talent.ApplyTalent(playerStats); // 应用天赋效果到玩家Stats上
        }
        else
        {
            // 当前槽位已有天赋，进行融合逻辑
            FuseTalents(currentTalent, talent);
        }
    }

    private void FuseTalents(Talent existingTalent, Talent newTalent)
    {
        Talent fusedTalent = null;

        // 检查是否是同一类型的基础天赋
        if (existingTalent.rarity == TalentRarity.Basic && newTalent.rarity == TalentRarity.Basic)
        {
            //if (existingTalent.type == newTalent.type)
            //{
            //    // 如果是同一类型的天赋，生成双字母天赋（例如SS）
            //    fusedTalent = CreateFusedTalent(GetDoubleLetterTalentType(existingTalent.type, existingTalent.type));
            //}
            //else
            //{
            //    // 不同类型的基础天赋，生成双字母天赋（例如S + N = SN）
                fusedTalent = CreateFusedTalent(GetDoubleLetterTalentType(existingTalent.type, newTalent.type));
            //}
        }
        else if (existingTalent.rarity == TalentRarity.Advanced && newTalent.rarity == TalentRarity.Basic)
        {
            // 基础单字母天赋与高阶天赋融合
            fusedTalent = CreateFusedTalent(existingTalent.type);

            // 50% 概率生成高阶天赋
            if (Random.Range(0, 100) < 50)
            {
                fusedTalent.rarity = TalentRarity.Elite;
            }
        }
        else if (existingTalent.rarity == TalentRarity.Elite && newTalent.rarity == TalentRarity.Basic)
        {
            // 基础单字母天赋与高阶天赋融合
            fusedTalent = CreateFusedTalent(newTalent.type);

            // 50% 概率生成高阶天赋
            if (Random.Range(0, 100) < 50)
            {
                fusedTalent.rarity = TalentRarity.Elite;
            }
        }

        // 95% 概率生成进阶天赋，5% 概率生成高阶天赋
        if (fusedTalent != null && fusedTalent.rarity == TalentRarity.Basic)
        {
            if (Random.Range(0, 100) < 5)
            {
                fusedTalent.rarity = TalentRarity.Elite;
            }
            else
            {
                fusedTalent.rarity = TalentRarity.Advanced;
            }
        }

        // 更新槽位中的天赋
        currentTalent = fusedTalent;
        //slotButton.image.sprite = filledSlotSprite; // 更新槽位图片
        //slotButton.image.sprite = filledSlotSprite; // 使用天赋图片或专属图片
        slotButton.image.color = currentTalent.color; // 使用天赋颜色
        slotButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = currentTalent.type.ToString(); // 显示天赋类型

        currentTalent.ApplyTalent(playerStats); // 应用新的天赋效果
    }

    private TalentType GetDoubleLetterTalentType(TalentType type1, TalentType type2)
    {
        int order1 = GetCustomOrder(type1);
        int order2 = GetCustomOrder(type2);

        string combinedType;
        if (order1 <= order2)
            combinedType = type1.ToString() + type2.ToString();
        else
            combinedType = type2.ToString() + type1.ToString();

        // 检查顺序并返回正确的TalentType
        switch (combinedType)
        {
            case "EI": return TalentType.EI;
            case "ES": return TalentType.ES;
            case "EN": return TalentType.EN;
            case "ET": return TalentType.ET;
            case "EF": return TalentType.EF;
            case "EJ": return TalentType.EJ;
            case "EP": return TalentType.EP;
            case "IS": return TalentType.IS;
            case "IN": return TalentType.IN;
            case "IT": return TalentType.IT;
            case "IF": return TalentType.IF;
            case "IJ": return TalentType.IJ;
            case "IP": return TalentType.IP;
            case "SN": return TalentType.SN;
            case "ST": return TalentType.ST;
            case "SF": return TalentType.SF;
            case "SJ": return TalentType.SJ;
            case "SP": return TalentType.SP;
            case "NT": return TalentType.NT;
            case "NF": return TalentType.NF;
            case "NJ": return TalentType.NJ;
            case "NP": return TalentType.NP;
            case "TF": return TalentType.TF;
            case "TJ": return TalentType.TJ;
            case "TP": return TalentType.TP;
            case "FJ": return TalentType.FJ;
            case "FP": return TalentType.FP;
            case "JP": return TalentType.JP;

            // 如果是相同的字母，返回双字母
            case "EE": return TalentType.EE;
            case "II": return TalentType.II;
            case "SS": return TalentType.SS;
            case "NN": return TalentType.NN;
            case "TT": return TalentType.TT;
            case "FF": return TalentType.FF;
            case "JJ": return TalentType.JJ;
            case "PP": return TalentType.PP;

            default:
                string errorMessage = $"Unexpected value: {combinedType}";
                Debug.LogError(errorMessage);  // 输出错误信息到控制台
                break;
        }

        return TalentType.Count;
    }

    private Talent CreateFusedTalent(TalentType fusedType)
    {
        // 根据融合后的类型创建一个新的天赋对象
        Talent fusedTalent = ScriptableObject.CreateInstance<Talent>();
        fusedTalent.type = fusedType;
        fusedTalent.rarity = TalentRarity.Advanced; // 默认设置为进阶天赋

        return fusedTalent;
    }

    //private Talent CreateAdvancedTalent(TalentType type)
    //{
    //    // 根据类型生成进阶天赋
    //    Talent advancedTalent = ScriptableObject.CreateInstance<Talent>();
    //    advancedTalent.type = type;
    //    advancedTalent.rarity = TalentRarity.Advanced;

    //    return advancedTalent;
    //}

    private int GetCustomOrder(TalentType type)
    {
        switch (type)
        {
            case TalentType.E: return 0;
            case TalentType.I: return 1;
            case TalentType.S: return 2;
            case TalentType.N: return 3;
            case TalentType.T: return 4;
            case TalentType.F: return 5;
            case TalentType.J: return 6;
            case TalentType.P: return 7;
            default: return int.MaxValue;
        }
    }
}

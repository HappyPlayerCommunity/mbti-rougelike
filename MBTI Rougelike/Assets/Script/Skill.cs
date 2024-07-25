using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 存放技能各种数据的脚本对象。简单技能的数据都储存于此。一些复杂的技能可能需要在Personality类中实现。
/// </summary>
[CreateAssetMenu(fileName = "NewSkillData", menuName = "Skill Data")]
public class Skill : ScriptableObject
{
    public enum SkillControlScheme
    {
        Continuous,         //按住按键，会持续自动使用技能，类似于机枪。
        ChargeRelease,      //按住按键为“蓄力”，松开时使用技能。
        Toggle,             //按下按键触发效果，再次释放需要松开并再次按下。
    }

    public enum RenderMode
    {
        HorizontalFlip,
        AllFlip,
    }

    [SerializeField, Tooltip("该技能生成的伤害块。")]
    private DamageCollider damageCollider;

    [SerializeField, Tooltip("该技能的冷却时间。")]
    private float reloadingTime = 1.0f;

    [SerializeField, Tooltip("该技能的散射角度，越大则弹道偏离准星的角度越远。")]
    private float scatterAngle = 0.15f;

    [SerializeField, Tooltip("该技能的发射出的伤害块的【移动/飞行速度】。")]
    private float damageColliderSpeed = 1.0f;

    [SerializeField, Tooltip("若技能为充能类型，该数值表示了技能可以储存的【最大充能数】。// 幸福蛋的蛋。")]
    private int maxChargeCount = 0;

    [SerializeField, Tooltip("该技能对自身产生的吹飞力，用于制作位移技能。")]
    private float selfBlowForce = 0.0f;

    [SerializeField, Tooltip("技能的操作模式，例如是按住连续释放，按住蓄力松开释放，或需要反复按下释放等。")]
    SkillControlScheme controlScheme;

    [SerializeField, Tooltip("伤害块的渲染模式，是水平翻转，或完全翻转。")]
    private RenderMode renderMode;

    public DamageCollider DamageCollider
    {
        get { return damageCollider; }
        set { damageCollider = value; }
    }

    public float ReloadingTime
    {
        get { return reloadingTime; }
        set { reloadingTime = value; }
    }

    public float ScatterAngle
    {
        get { return scatterAngle; }
        set { scatterAngle = value; }
    }

    public int MaxChargeCount
    {
        get { return maxChargeCount; }
        set { maxChargeCount = value; }
    }

    public float DamageColliderSpeed
    {
        get { return damageColliderSpeed; }
        set { damageColliderSpeed = value; }
    }

    public RenderMode GetRenderMode
    {
        get { return renderMode; }
        set { renderMode = value; }
    }

    public float SelfBlowForce
    {
        get { return selfBlowForce; }
        set { selfBlowForce = value; }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 存放角色的人格倾向值。这些数值决定了玩家的人格模型，以及其他能力值的基础值。
/// </summary>
[CreateAssetMenu(fileName = "NewPreferenceData", menuName = "Preference Data")]
public class Preference : ScriptableObject
{
    [Tooltip("【外倾】：与火，移速，韧性，羁绊移速，攻击范围，攻击充能率有关。")]
    public int Extraversion = 5;

    [Tooltip("【内倾】：与冰，护盾上限，护盾恢复率，建筑耐久，击退，受伤充能率有关。")]
    public int Introversion = 5;

    [Tooltip("【实感】：与地，生命上限，生命再生，经验倍率，攻速，实体攻击力有关。")]
    public int Sensing = 5;

    [Tooltip("【直觉】：与风，闪避率，自动充能率，幸运，特技冷却，抽象攻击力有关。")]
    public int Intuition = 5;

    [Tooltip("【思维】：与雷，全局攻击力有关。")]
    public int Thinking = 5;

    [Tooltip("【情感】：与水，治疗力有关。")]
    public int Feeling = 5;

    [Tooltip("【决断】：与暴击，暴击伤害，建筑强度，和羁绊强度有关。")]
    public int Judging = 5;

    [Tooltip("【展望】：与异常强度，异常持续时间，异常伤害，异常抗性有关。")]
    public int Perceiving = 5;
}

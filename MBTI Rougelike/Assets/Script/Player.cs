﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家类。继承自单位，用来实现一些独属于玩家的功能。
/// </summary>
public class Player : Unit
{
    [Header("互动组件")]

    [Tooltip("玩家Art的Transform。")]
    public Transform playerArtTransform;

    [Tooltip("武器Art的Transform。")]
    public Transform weaponArtTransform;

    [Tooltip("武器Art的Transform。")]
    public Personality personality;


    public override void TakeDamage(int damage, float stuntime)
    {
        base.TakeDamage(damage, stuntime);

        personality.ChargeEnerge(damage); // 受伤充能比率还得具体设计。
    }
}

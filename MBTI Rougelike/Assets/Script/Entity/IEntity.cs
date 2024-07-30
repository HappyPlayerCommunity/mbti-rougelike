using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 所有实体的接口。
/// </summary>
public interface IEntity
{
    void TakeDamage(int damage, float stunTimer);
}
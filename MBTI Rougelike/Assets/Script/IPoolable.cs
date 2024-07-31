using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolable
{
    /// <summary>
    /// 从对象池中复用对象时，重置目标的一系列状态（如持续时间）。
    /// </summary>
    void ResetObjectState();
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolable
{
    /// <summary>
    /// 从对象池中复用对象时，重置目标的一系列状态（如持续时间）。
    /// </summary>
    void ResetObjectState();

    /// <summary>
    /// 从对象池中复用对象后，用通用的位置和旋转激活对象。
    /// </summary>
    void Activate(Vector3 position, Quaternion rotation);

    /// <summary>
    /// 将对象回收到对象池时调用，这个方法理论上应当覆盖所有的Destroy()方法。
    /// </summary>
    void Deactivate();

    /// <summary>
    /// 用于对象池字典的标识。
    /// </summary>
    string PoolKey { get; set; }
}

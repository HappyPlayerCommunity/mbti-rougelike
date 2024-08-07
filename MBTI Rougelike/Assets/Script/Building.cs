using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : BaseEntity
{

    protected override void FixedUpdate()
    {
        //常规Building不需要移动，所以不需要调用基类的FixedUpdate
    }
}

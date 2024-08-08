using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : BaseEntity
{

    protected override void FixedUpdate()
    {
        //常规Building不需要移动，所以不需要调用基类的FixedUpdate

        //通过添加逻辑，也可以制作能被击飞，或是移动的建筑/障碍物。
    }
}

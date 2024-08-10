using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 顺序不能随意更改，需要和编辑器顺序同步。
public enum Animation
{
    ShieldBreak = 0,
    ShieldRestore = 1
}

/// <summary>
/// 单例模式的动画管理器。各个类可以用此类来播放一些通用动画。
/// </summary>
public class AnimationManager : MonoBehaviour
{
    public static AnimationManager Instance { get; private set; }

    public List<AnimationController2D> animationController2Ds;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayAnimation(Animation animation, Transform attachedTransform, bool attached = false)
    {
        //var anim = Instantiate(animationController2Ds[(int)animation], attachedTransform.position, Quaternion.identity);

        var animPrefab = animationController2Ds[(int)animation];
        GameObject animObj = PoolManager.Instance.GetObject(animPrefab.name, animPrefab.gameObject);
        AnimationController2D anim = animObj.GetComponent<AnimationController2D>();
        anim.Activate(attachedTransform.position, Quaternion.identity);

        if (attached)
        {
            anim.isAttached = true;
            anim.attachedTransform = attachedTransform;
        }
    }
}
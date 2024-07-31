using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 用来控制所有2D帧动画的类。
/// </summary>
public class AnimationController2D : MonoBehaviour
{
    protected Animator animator;
    public bool isAttached = false;
    public Transform attachedTransform;
    private bool animationFinished = false;

    public Animator GetAnimator()
    {
        return animator;
    }

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (isAttached)
        {
            transform.position = attachedTransform.position;
        }
    }

    public void PlayAnimation(string name)
    {
        animator.Play(name);
    }

    public void OnAnimationEnd()
    {
        animationFinished = true;
        Destroy(gameObject);
    }
}

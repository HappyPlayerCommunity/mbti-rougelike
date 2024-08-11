using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UltraSun : MonoBehaviour
{
    [SerializeField, Tooltip("移动到高空的时间。")]
    private float moveDuration = 2.0f;

    [SerializeField, Tooltip("造成伤害的间隔。")]
    private float effectInterval = 1.0f;

    [SerializeField, Tooltip("太阳持续时间。")]
    public float duration = 5.0f;

    [SerializeField, Tooltip("太阳发光前的等待时间。")]
    public float wait = 2.0f;

    [SerializeField, Tooltip("太阳的每段伤害。")]
    public int damageAmount = 10;

    [SerializeField, Tooltip("太阳的每段治疗。")]
    public int healAmount = 10;

    [SerializeField, Tooltip("太阳伤害的硬直时间。")]
    public float stunTime = 0.25f;

    private Vector3 targetPosition;
    private float elapsedTime = 0.0f;
    private bool isEffectActive = false;

    public Vector2 sunPosition = new Vector2(0.5f, 0.85f);


    [SerializeField, Tooltip("太阳开始发光时的动画。")]
    public AnimationController2D sunAnim;

    [SerializeField, Tooltip("太阳击中敌人时的动画。")]
    public AnimationController2D hitAnim;

    [SerializeField, Tooltip("太阳回复友军时的动画。")]
    public AnimationController2D healAnim;

    Player player;


    void Start()
    {
        // 计算目标位置（屏幕中心上方）
        targetPosition = Camera.main.ViewportToWorldPoint(new Vector3(sunPosition.x, sunPosition.y, Camera.main.nearClipPlane));
        targetPosition.z = 0;

        // 开始移动到目标位置
        StartCoroutine(MoveToTarget());

        player = GameObject.FindObjectOfType<Player>();
    }

    private void Update()
    {
        if (isEffectActive)
        {
            targetPosition = Camera.main.ViewportToWorldPoint(new Vector3(sunPosition.x, sunPosition.y, Camera.main.nearClipPlane));
            targetPosition.z = 0;
            transform.position = targetPosition;
        }
    }

    IEnumerator MoveToTarget()
    {
        Vector3 startPosition = transform.position;
        float moveElapsedTime = 0.0f;

        while (moveElapsedTime < moveDuration)
        {
            moveElapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(startPosition, targetPosition, moveElapsedTime / moveDuration);
            yield return null;
        }

        // 移动完成后，开始效果
        isEffectActive = true;
        StartCoroutine(ApplyEffects());
    }

    IEnumerator ApplyEffects()
    {
        yield return new WaitForSeconds(wait);

        if (sunAnim)
        {
            GameObject hitEffect = PoolManager.Instance.GetObject(sunAnim.name, sunAnim.gameObject);
            AnimationController2D anim = hitEffect.GetComponent<AnimationController2D>();
            anim.Activate(transform.position, Quaternion.identity);
        }

        while (elapsedTime < duration)
        {
            elapsedTime += effectInterval;

            // 对所有敌人造成伤害
            ApplyDamageToEnemies();

            // 对所有友军造成治疗
            ApplyHealToAllies();

            yield return new WaitForSeconds(effectInterval);
        }

        // 效果结束后销毁自身
        Destroy(gameObject);
    }


    //目前，超级太阳不会造成暴击。

    void ApplyDamageToEnemies()
    {
        // 查找所有敌人并对其造成伤害
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemyObj in enemies)
        {
            var enemy = enemyObj.GetComponent<Enemy>();

            //后续补全：伤害类型，暴击等

            if (enemy != null)
            {
                DamagePopupManager.Instance.Popup(PopupType.Damage, enemy.transform.position, damageAmount, false);
                enemy.TakeDamage(damageAmount, stunTime);

                if (hitAnim)
                {
                    GameObject hitEffect = PoolManager.Instance.GetObject(hitAnim.name, hitAnim.gameObject);
                    AnimationController2D anim = hitEffect.GetComponent<AnimationController2D>();
                    anim.Activate(enemy.transform.position, Quaternion.identity);
                }
            }
        }
    }

    void ApplyHealToAllies()
    {
        // 羁绊系统实现之后补全治疗效果，现在只治疗玩家。
        //GameObject[] allies = GameObject.FindGameObjectsWithTag("Ally");

        if (player)
        {
            DamagePopupManager.Instance.Popup(PopupType.Healing, player.transform.position, healAmount, false);
            player.GetHealing(healAmount);

            if (healAnim)
            {
                GameObject hitEffect = PoolManager.Instance.GetObject(healAnim.name, healAnim.gameObject);
                AnimationController2D anim = hitEffect.GetComponent<AnimationController2D>();
                anim.attachedTransform = player.transform;
                anim.Activate(player.transform.position, Quaternion.identity);
            }
        }
    }
}

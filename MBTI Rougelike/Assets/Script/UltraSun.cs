using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    void Start()
    {
        // 计算目标位置（屏幕中心上方）
        targetPosition = Camera.main.ViewportToWorldPoint(new Vector3(sunPosition.x, sunPosition.y, Camera.main.nearClipPlane));
        targetPosition.z = 0;

        // 开始移动到目标位置
        StartCoroutine(MoveToTarget());
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

        AnimationManager.Instance.PlayAnimation(Animation.UltraSun, transform);

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
            if (enemy != null)
            {
                DamagePopupManager.Instance.Popup(PopupType.Damage, enemy.transform.position, damageAmount, false);
                enemy.TakeDamage(damageAmount, stunTime);
            }
        }
    }

    void ApplyHealToAllies()
    {
        // 羁绊系统实现之后补全治疗效果，现在只治疗玩家。
        //GameObject[] allies = GameObject.FindGameObjectsWithTag("Ally");

        Player player = GameObject.FindObjectOfType<Player>();
        if (player)
        {
            DamagePopupManager.Instance.Popup(PopupType.Healing, player.transform.position, healAmount, false);
            player.GetHealing(healAmount);
        }
    }
}

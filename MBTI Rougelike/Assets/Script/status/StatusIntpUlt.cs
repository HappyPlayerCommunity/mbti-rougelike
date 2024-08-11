using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "NewIntpStatus", menuName = "Status Data/Intp Ult Data")]
public class StatusIntpUlt : Status
{
    public Skill skill; // 需要技能参考，发射的auto的子弹。
    public Player player;
    public float fireRate = 0.1f; // 每次发射的间隔时间
    public int totalCircles = 5; // 总共旋转的圈数

    private float toungness = float.MaxValue;

    public override void OnUpdate(GameObject target, float deltaTime)
    {
        base.OnUpdate(target, deltaTime);
    }

    public override void OnApply(GameObject target)
    {
        base.OnApply(target);

        MonoBehaviour monoBehaviour = target.GetComponent<MonoBehaviour>();
        player = target.GetComponent<Player>();

        if (monoBehaviour != null)
        {
            monoBehaviour.StartCoroutine(FireBullets());
        }
        stats.toughness += toungness;
        player.StatsUpdate();
    }

    public override void OnExpire(GameObject target)
    {
        base.OnExpire(target);
        stats.toughness -= toungness;
        var player = target.GetComponent<Player>();
        player.StatsUpdate();
    }

    private IEnumerator FireBullets()
    {
        float totalRotation = 0f;

        // 计算每秒需要的旋转速度，确保在 duration 时间内完成 totalCircles 圈
        float angleStepPerSecond = (360f * totalCircles) / duration;

        GameObject tempObj1 = new GameObject("Temp1");
        GameObject tempObj2 = new GameObject("Temp2");

        List<Transform> multiInitPos = new List<Transform>
        {
            tempObj1.transform,
            tempObj2.transform
        };

        float startTime = Time.time;

        while (Time.time - startTime < duration)
        {
            // 计算当前的发射方向
            float currentAngle = totalRotation % 360f;
            Vector3 aimDirection = Quaternion.Euler(0, 0, currentAngle) * Vector3.right;

            // 确定子弹之间的偏移距离
            float bulletSpacing = Vector3.Distance(player.personality.normalAttack_MultiInitPositions[0].position, player.personality.normalAttack_MultiInitPositions[1].position) / 2f;

            // 从玩家的中心位置开始计算发射点
            Vector3 offset1 = player.transform.right * -bulletSpacing;
            Vector3 offset2 = player.transform.right * bulletSpacing;

            // 将偏移应用到player的transform上
            multiInitPos[0].position = player.transform.position + offset1;
            multiInitPos[1].position = player.transform.position + offset2;

            for (int i = 0; i < skill.MultiDamageColliders.Count; i++)
            {
                DamageCollider damageCollider = skill.MultiDamageColliders[i];
                Transform transform = multiInitPos[i];
                var finalDamageCollider = AttackHelper.InitDamageCollider(damageCollider, transform, 0.0f, aimDirection, 0.0f, skill.ControlScheme, skill.FixPos, 1.0f, skill.GetRenderMode, player, skill.DamageColliderSpeed);
            }

            // 根据经过的时间来更新总旋转角度
            float deltaTime = Time.deltaTime;
            totalRotation += angleStepPerSecond * deltaTime;

            yield return null;
        }

        // 清理临时创建的 GameObject
        GameObject.Destroy(tempObj1);
        GameObject.Destroy(tempObj2);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunFlower : Unit
{
    [SerializeField, Tooltip("第一次生成蝴蝶前的时间间隔。")]
    private float firstWaitTime = 1.0f;

    [SerializeField, Tooltip("生成蝴蝶的时间间隔。")]
    private float spawnInterval = 5.0f;

    [SerializeField, Tooltip("蝴蝶的预制体。")]
    private GameObject butterflyPrefab;

    [SerializeField, Tooltip("蝴蝶的生成位置。")]
    private Transform spawnPoint;

    [SerializeField, Tooltip("蝴蝶的技能信息。")]
    private Skill skill;

    Player player;

    protected override void Start()
    {
        base.Start();
        toughness = 0.0f;
        player = GameObject.FindObjectOfType<Player>();
        StartCoroutine(SpawnButterfly());
    }

    private IEnumerator SpawnButterfly()
    {
        yield return new WaitForSeconds(firstWaitTime);
        while (true)
        {
            AttackHelper.InitSkillDamageCollider(skill, spawnPoint, 1.0f, player, 0.0f, Vector3.left + Vector3.up * 0.6f, 0.0f, player);
            AttackHelper.InitSkillDamageCollider(skill, spawnPoint, 1.0f, player, 0.0f, Vector3.up, 0.0f, player);
            AttackHelper.InitSkillDamageCollider(skill, spawnPoint, 1.0f, player, 0.0f, Vector3.right + Vector3.up * 0.6f, 0.0f, player);

            yield return new WaitForSeconds(spawnInterval);
        }
    }
}

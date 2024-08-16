using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class EnemiesList
{
    public List<Enemy> enemies;
}

/// <summary>
/// 用来管理所有敌人的类。目前只有简单的波数功能。后续应该要添加共同的AI，大目标等功能。
/// </summary>
public class EnemyManager : MonoBehaviour
{
    [SerializeField, Tooltip("每一波敌人的详细信息。")]
    public List<EnemiesList> enemyWavesInfo;

    [SerializeField, Tooltip("每波内生成敌人的间隔。")]
    public float betweenWavesInterval = 5.0f; 

    [SerializeField, Tooltip("每波内生成敌人的间隔。")]
    public float spawnInterval = 1.0f;

    [SerializeField, Tooltip("当前波数。")]
    private int currentWave = 0;

    private List<GameObject> currentWaveEnemies = new List<GameObject>();
    private Coroutine spawnWaveCoroutine;

    [SerializeField, Tooltip("ResetTesting 按钮")]
    private Button resetTestingButton;

    [SerializeField, Tooltip("在动画播放结束的前【X】秒就召唤敌人。")]
    private float aheadsummonTime = 0.3f;

    [SerializeField, Tooltip("怪物召唤动画")]
    AnimationController2D summonEffectPrefab;

    void Start()
    {
        currentWave = 0; // 确保从第一波开始
        spawnWaveCoroutine = StartCoroutine(SpawnWave());
    }

    public void ResetTesting()
    {
        List<GameObject> enemiesToRemove = new List<GameObject>(currentWaveEnemies);

        foreach (var obj in enemiesToRemove)
        {
            obj.GetComponent<Unit>().Die();
        }

        currentWaveEnemies.Clear();
        currentWave = 0;

        if (spawnWaveCoroutine != null)
        {
            StopCoroutine(spawnWaveCoroutine);
        }

        spawnWaveCoroutine = StartCoroutine(SpawnWave());
    }

    IEnumerator SpawnWave()
    {
        while (currentWave < enemyWavesInfo.Count)
        {
            if (resetTestingButton != null)
            {
                resetTestingButton.interactable = false;
            }

            EnemiesList currentWaveInfo = enemyWavesInfo[currentWave];

            foreach (var enemy in currentWaveInfo.enemies)
            {
                StartCoroutine(SpawnEnemyWithDelay(enemy));
                yield return new WaitForSeconds(spawnInterval);
            }

            // 等待当前波的所有敌人被消灭
            yield return new WaitUntil(() => currentWaveEnemies.Count == 0);

            if (resetTestingButton != null)
            {
                resetTestingButton.interactable = true;
            }

            // 等待 betweenWavesInterval 时间后再生成下一波敌人
            yield return new WaitForSeconds(betweenWavesInterval);
            currentWave++;
        }
    }

    IEnumerator SpawnEnemyWithDelay(Enemy enemyPrefab)
    {
        Camera camera = Camera.main;
        Vector3 screenBottomLeft = camera.ViewportToWorldPoint(new Vector3(0, 0, camera.nearClipPlane));
        Vector3 screenTopRight = camera.ViewportToWorldPoint(new Vector3(1, 1, camera.nearClipPlane));

        float width = screenTopRight.x - screenBottomLeft.x;
        float height = screenTopRight.y - screenBottomLeft.y;

        float randomX = Random.Range(screenBottomLeft.x, screenTopRight.x);
        float randomY = Random.Range(screenBottomLeft.y, screenTopRight.y);
        Vector3 spawnPosition = new Vector3(randomX, randomY, 0.0f);

        GameObject enemyObj = PoolManager.Instance.GetObject(enemyPrefab.name, enemyPrefab.gameObject);
        Enemy enemy = enemyObj.GetComponent<Enemy>();
        currentWaveEnemies.Add(enemy.gameObject);

        if (summonEffectPrefab)
        {
            GameObject effect = PoolManager.Instance.GetObject(summonEffectPrefab.name, summonEffectPrefab.gameObject);
            AnimationController2D anim = effect.GetComponent<AnimationController2D>();
            anim.Activate(spawnPosition, Quaternion.identity);
            yield return new WaitForSeconds(0.1f); // Unity需要一个缓冲时间启动动画
            yield return new WaitForSeconds(anim.GetRemainingAnimationTime() - aheadsummonTime);
        }
        else
        {
            yield return new WaitForSeconds(2.0f); // 随便hardcode一个时间
        }

        // 等待召唤动画播放完毕

        enemy.Activate(spawnPosition, Quaternion.identity);
        enemy.GetComponent<Enemy>().OnEnemyDeath += () => RemoveEnemyFromList(enemy.gameObject);
    }

    void RemoveEnemyFromList(GameObject enemy)
    {
        if (currentWaveEnemies.Contains(enemy))
        {
            currentWaveEnemies.Remove(enemy);
        }
    }
}

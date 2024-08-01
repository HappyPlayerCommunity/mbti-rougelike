using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 用来管理所有敌人的类。目前只有简单的波数功能。后续应该要添加共同的AI，大目标等功能。
/// </summary>
public class EnemyManager : MonoBehaviour
{
    [SerializeField, Tooltip("生成的敌人类型。")]
    public GameObject enemyPrefab; //后续应该设置一个波的list，可以自由分配敌人各个类型的数量，然后拖进来。

    [SerializeField, Tooltip("每波生成多少敌人。")]
    public int enemyPerWave = 5;

    [SerializeField, Tooltip("每波内生成敌人的间隔。")]
    public float betweenWavesInterval = 3.0f;

    [SerializeField, Tooltip("每波内生成敌人的间隔。")]
    public float spawnInterval = 1.0f;

    [SerializeField, Tooltip("一共多少波。")]
    public int totalWaves = 3;

    [SerializeField, Tooltip("当前波数。")]
    private int currentWave = 0;

    private List<GameObject> currentWaveEnemies = new List<GameObject>();
    private Coroutine spawnWaveCoroutine;

    void Start()
    {
        spawnWaveCoroutine = StartCoroutine(SpawnWave());
    }

    public void ResetTesting()
    {
        foreach (var obj in currentWaveEnemies)
        {
            //Destroy(obj);
            obj.GetComponent<Enemy>().Deactivate();
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
        while (currentWave < totalWaves)
        {
            for (int i = 0; i < enemyPerWave; i++)
            {
                SpawnEnemy();
                yield return new WaitForSeconds(spawnInterval);
            }

            yield return new WaitUntil(() => currentWaveEnemies.Count == 0);
            yield return new WaitForSeconds(betweenWavesInterval);
            currentWave++;
        }
    }

    void SpawnEnemy()
    {
        Camera camera = Camera.main;
        Vector3 screenBottomLeft = camera.ViewportToWorldPoint(new Vector3(0, 0, camera.nearClipPlane));
        Vector3 screenTopRight = camera.ViewportToWorldPoint(new Vector3(1, 1, camera.nearClipPlane));

        float width = screenTopRight.x - screenBottomLeft.x;
        float height = screenTopRight.y - screenBottomLeft.y;

        float randomX = Random.Range(screenBottomLeft.x, screenTopRight.x);
        float randomY = Random.Range(screenBottomLeft.y, screenTopRight.y);
        Vector3 spawnPosition = new Vector3(randomX, randomY, 0.0f);

        //GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

        GameObject enemyObj = PoolManager.Instance.GetObject(enemyPrefab.name, enemyPrefab.gameObject);
        Enemy enemy = enemyObj.GetComponent<Enemy>();
        enemy.Activate(spawnPosition, Quaternion.identity);

        currentWaveEnemies.Add(enemy.gameObject);

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
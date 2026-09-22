using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {
    public static Spawner Instance { get; private set; }
    [SerializeField] private Transform spawnPointHolder;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private SpawnerData spawnerData;

    public SpawnerData Data => spawnerData;

    private Queue<EnemyController> enemyPool = new Queue<EnemyController>();
    private float nextSpawnTime;
    private int activeEnemyCount = 0;
    private PlayerController player;
    private Transform target;

    public float SurvivalTimer { get; private set; }
    public int CurrentSpawnCount { get; private set; } = 1;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        InitializePool();
        player = FindAnyObjectByType<PlayerController>();
        if (target != null)
        {
            target.position = player.gameObject.transform.position;
        }
    }

    private void Update()
    {
        SurvivalTimer += Time.deltaTime;

        UpdateDifficulty();

        if (spawnPointHolder != null && player != null)
        {
            spawnPointHolder.position = player.transform.position;
        }

        if (activeEnemyCount >= spawnerData.maxEnemiesAlive) return;


        if (Time.time >= nextSpawnTime)
        {
            SpawnWave();
            nextSpawnTime = Time.time + spawnerData.spawnInterval;
        }
    }

    private void UpdateDifficulty()
    {
        if (spawnerData == null || spawnerData.timeToIncreaseDifficulty <= 0) return;

        int stagesPassed = Mathf.FloorToInt(SurvivalTimer / spawnerData.timeToIncreaseDifficulty);

        int calculatedCount = spawnerData.baseSpawnCount + (stagesPassed * spawnerData.spawnCountIncrement);

        CurrentSpawnCount = Mathf.Min(calculatedCount, spawnerData.maxSpawnCount);
    }

    private void SpawnWave()
    {
        for (int i = 0; i < CurrentSpawnCount; i++)
        {
            SpawnEnemyFromPool();
        }
    }

    private void InitializePool()
    {
        if (spawnerData == null || spawnerData.enemyPrefab == null) return;
        for (int i = 0; i < spawnerData.initialPoolSize; i++)
        {
            CreateNewEnemyForPool();
        }
    }

    private EnemyController CreateNewEnemyForPool()
    {
        GameObject go = Instantiate(spawnerData.enemyPrefab, transform);
        EnemyController enemy = go.GetComponent<EnemyController>();
        go.SetActive(false);
        enemyPool.Enqueue(enemy);
        return enemy;
    }

    public void SpawnEnemyFromPool()
    {
        if (spawnPoints == null || spawnPoints.Length == 0) return;

        if (enemyPool.Count == 0)
        {
            CreateNewEnemyForPool();
        }

        EnemyController enemy = enemyPool.Dequeue();

        Transform chosenPoint = GetSmartSpawnPoint();
        Vector2 spawnOffset = Random.insideUnitCircle * 0.4f;
        Vector2 finalPos = (Vector2)chosenPoint.position + spawnOffset;
        enemy.gameObject.SetActive(true);
        enemy.ResetEnemy(finalPos);
        activeEnemyCount++;
    }

    private Transform GetSmartSpawnPoint()
    {
        if (player == null) return spawnPoints[Random.Range(0, spawnPoints.Length)];
        Vector2 moveDir = player.RB.linearVelocity.normalized;
        if (moveDir.sqrMagnitude < 0.01f)
        {
            return spawnPoints[Random.Range(0, spawnPoints.Length)];
        }
        List<Transform> forwardSpawnPoints = new List<Transform>();
        Vector2 playerPos = player.transform.position;
        foreach (Transform pt in spawnPoints)
        {
            Vector2 dirToPoint = ((Vector2)pt.position - playerPos).normalized;
            if (Vector2.Dot(dirToPoint, moveDir) > 0.1f)
            {
                forwardSpawnPoints.Add(pt);
            }
        }
        if (forwardSpawnPoints.Count > 0)
        {
            return forwardSpawnPoints[Random.Range(0, forwardSpawnPoints.Count)];
        }
        return spawnPoints[Random.Range(0, spawnPoints.Length)];
    }

    public void ReturnEnemyToPool(EnemyController enemy)
    {
        enemy.Core.Movement. SetVelocityZero();
        enemy.gameObject.SetActive(false);
        enemyPool.Enqueue(enemy);
        activeEnemyCount--;
    }
}

using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {
    public static Spawner Instance { get; private set; }

    [SerializeField] private Transform spawnPointHolder;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private SpawnerData spawnerData;
    [SerializeField] private WaveData waveData;

    [SerializeField] private float debugStartTime = 0f;

    public SpawnerData Data => spawnerData;
    public float SurvivalTimer { get; private set; }
    private readonly Dictionary<GameObject, Queue<EnemyController>> pools = new Dictionary<GameObject, Queue<EnemyController>>();
    private readonly Dictionary<GameObject, int> activeByPrefab = new Dictionary<GameObject, int>();
    private readonly Dictionary<EnemyController, GameObject> prefabOf = new Dictionary<EnemyController, GameObject>();

    private float[][] nextSpawnTimes;
    private bool[] waveStarted;

    private int activeEnemyCount;
    private PlayerController player;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        player = FindAnyObjectByType<PlayerController>();
        SurvivalTimer = debugStartTime;

        BuildWaveRuntimeData();
        InitializePools();
    }

    private void Update()
    {
        SurvivalTimer += Time.deltaTime;

        if (spawnPointHolder != null && player != null)
        {
            spawnPointHolder.position = player.transform.position;
        }

        UpdateWaves();
    }

    private void BuildWaveRuntimeData()
    {
        if (waveData == null)
        {
            return;
        }

        int waveCount = waveData.waves.Count;
        nextSpawnTimes = new float[waveCount][];
        waveStarted = new bool[waveCount];

        for (int w = 0; w < waveCount; w++)
        {
            WaveEntry wave = waveData.waves[w];
            nextSpawnTimes[w] = new float[wave.enemies.Count];

            for (int e = 0; e < wave.enemies.Count; e++)
            {
                nextSpawnTimes[w][e] = wave.startTime;
            }
        }
    }

    private void UpdateWaves()
    {
        if (nextSpawnTimes == null) return;

        List<WaveEntry> waves = waveData.waves;

        for (int w = 0; w < waves.Count && w < nextSpawnTimes.Length; w++)
        {
            WaveEntry wave = waves[w];
            if (!wave.IsActiveAt(SurvivalTimer)) continue;

            if (!waveStarted[w])
            {
                waveStarted[w] = true;
            }

            for (int e = 0; e < wave.enemies.Count && e < nextSpawnTimes[w].Length; e++)
            {
                EnemySpawnInfo info = wave.enemies[e];
                if (info.enemyPrefab == null) continue;
                if (SurvivalTimer < nextSpawnTimes[w][e]) continue;

                SpawnBatch(info);
                nextSpawnTimes[w][e] = SurvivalTimer + Mathf.Max(0.05f, info.spawnInterval);
            }
        }
    }

    private void SpawnBatch(EnemySpawnInfo info)
    {
        for (int i = 0; i < info.countPerSpawn; i++)
        {
            if (activeEnemyCount >= spawnerData.maxEnemiesAlive) return;
            if (info.maxAlive > 0 && GetActiveCount(info.enemyPrefab) >= info.maxAlive) return;

            SpawnEnemyFromPool(info.enemyPrefab);
        }
    }


    private Queue<EnemyController> GetPool(GameObject prefab)
    {
        if (!pools.TryGetValue(prefab, out Queue<EnemyController> pool))
        {
            pool = new Queue<EnemyController>();
            pools[prefab] = pool;
            activeByPrefab[prefab] = 0;
        }
        return pool;
    }

    private int GetActiveCount(GameObject prefab)
    {
        return activeByPrefab.TryGetValue(prefab, out int count) ? count : 0;
    }

    private void InitializePools()
    {
        if (waveData == null || spawnerData == null) return;

        var seen = new HashSet<GameObject>();
        foreach (WaveEntry wave in waveData.waves)
        {
            foreach (EnemySpawnInfo info in wave.enemies)
            {
                if (info.enemyPrefab == null || !seen.Add(info.enemyPrefab)) continue;

                for (int i = 0; i < spawnerData.initialPoolSize; i++)
                {
                    CreateNewEnemyForPool(info.enemyPrefab);
                }
            }
        }
    }

    private EnemyController CreateNewEnemyForPool(GameObject prefab)
    {
        GameObject go = Instantiate(prefab, transform);
        EnemyController enemy = go.GetComponent<EnemyController>();

        if (enemy == null)
        {
            Destroy(go);
            return null;
        }

        go.SetActive(false);
        prefabOf[enemy] = prefab;
        GetPool(prefab).Enqueue(enemy);
        return enemy;
    }

    private void SpawnEnemyFromPool(GameObject prefab)
    {
        if (spawnPoints == null || spawnPoints.Length == 0) return;

        Queue<EnemyController> pool = GetPool(prefab);
        if (pool.Count == 0)
        {
            CreateNewEnemyForPool(prefab);
            if (pool.Count == 0) return;
        }

        EnemyController enemy = pool.Dequeue();

        Transform chosenPoint = GetSmartSpawnPoint();
        Vector2 spawnOffset = Random.insideUnitCircle * 0.4f;
        Vector2 finalPos = (Vector2)chosenPoint.position + spawnOffset;

        enemy.gameObject.SetActive(true);
        enemy.ResetEnemy(finalPos);

        activeEnemyCount++;
        activeByPrefab[prefab]++;
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

        if (!enemy.gameObject.activeSelf) return;

        enemy.Core.Movement.SetVelocityZero();
        enemy.gameObject.SetActive(false);

        if (prefabOf.TryGetValue(enemy, out GameObject prefab))
        {
            GetPool(prefab).Enqueue(enemy);  
            activeByPrefab[prefab]--;
        }
        activeEnemyCount--;
    }
}
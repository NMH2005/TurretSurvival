using UnityEngine;

[CreateAssetMenu(fileName = "SpawnerData", menuName = "Scriptable Objects/SpawnerData")]
public class SpawnerData : ScriptableObject
{
    public GameObject enemyPrefab;
    public float spawnInterval = 1.5f;
    public int initialPoolSize = 30;
    public int baseSpawnCount = 1;
    public float timeToIncreaseDifficulty = 60f;
    public int spawnCountIncrement = 1;
    public int maxSpawnCount = 10;
    public int maxEnemiesAlive = 100;
    public float despawnDistance = 22f;
}
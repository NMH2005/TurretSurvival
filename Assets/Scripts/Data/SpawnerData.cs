using UnityEngine;

[CreateAssetMenu(fileName = "SpawnerData", menuName = "Scriptable Objects/SpawnerData")]
public class SpawnerData : ScriptableObject
{
    public int initialPoolSize = 30;
    public int maxEnemiesAlive = 100;
    public float despawnDistance = 22f;
}
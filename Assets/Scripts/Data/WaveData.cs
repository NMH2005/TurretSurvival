using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "WaveData", menuName = "Scriptable Objects/WaveData")]

public class WaveData : ScriptableObject
{
    public List<WaveEntry> waves = new List<WaveEntry>();

    private void OnValidate()
    {
        if (waves == null) return;
        foreach (var wave in waves)
        {
            wave?.Validate();
        }
    }
}

[Serializable]
public class WaveEntry {
    public string name = "Wave";

    [Min(0f)] public float startTime = 0f;

    public float endTime = 60f;

    public List<EnemySpawnInfo> enemies = new List<EnemySpawnInfo>();

    public bool IsActiveAt(float time)
    {
        return time >= startTime && (endTime < 0f || time < endTime);
    }

    public void Validate()
    {
        if (endTime >= 0f && endTime < startTime) endTime = startTime;
    }
}

[Serializable]
public class EnemySpawnInfo {
    public GameObject enemyPrefab;

    [Min(1)] public int countPerSpawn = 1;

    [Min(0.05f)] public float spawnInterval = 2f;

    [Min(0)] public int maxAlive = 0;
}

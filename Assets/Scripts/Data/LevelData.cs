using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Scriptable Objects/LevelData")]
public class LevelData : ScriptableObject
{
    public int baseExpToLevel = 100;
    public float growthMultiplier = 1.2f;
    public int flatIncrementPerLevel = 10;
    public int GetExpRequired(int currentLevel)
    {
        float scaled = baseExpToLevel * Mathf.Pow(growthMultiplier, currentLevel - 1);
        return Mathf.RoundToInt(scaled) + (flatIncrementPerLevel * (currentLevel - 1));
    }
}

using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Scriptable Objects/LevelData")]
public class LevelData : ScriptableObject
{
    public int baseExpToLevel = 100;
    public float growthMultiplier = 1.2f;
    public int flatIncrementPerLevel = 10;
    public int GetExpRequired(int currentLevel)
    {
        double scaled = baseExpToLevel * System.Math.Pow(growthMultiplier, currentLevel - 1);
        scaled = System.Math.Round(scaled) + flatIncrementPerLevel * (double)(currentLevel - 1);
        return (int)System.Math.Clamp(scaled, 1d, int.MaxValue);
    }
}

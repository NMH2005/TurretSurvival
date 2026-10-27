using UnityEngine;

public enum UpgradeType 
{
    MainDamage,
    MainFireInterval,
    Recovery
}

[CreateAssetMenu(fileName = "UpgradeData", menuName = "Scriptable Objects/UpgradeData")]
public class UpgradeData : ScriptableObject {
    public string displayName;
    public string description;
    public Sprite icon;
    public UpgradeType type;
    public int maxRank = 3;
    public float value = 0.1f;
}

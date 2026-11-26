using UnityEngine;

public enum UpgradeType 
{
    MainDamage,
    MainFireInterval,
    Recovery,
    AcquireAutomatic = 3,
    AutomaticDamage = 4,
    AutomaticCooldown = 5
}

[CreateAssetMenu(fileName = "UpgradeData", menuName = "Scriptable Objects/UpgradeData")]
public class UpgradeData : ScriptableObject {
    public WeaponData targetWeapon;
    public string displayName;
    public string description;
    public Sprite icon;
    public UpgradeType type;
    [Min(0), Tooltip("0 means unlimited ranks.")]
    public int maxRank;
    public float value = 0.1f;
}

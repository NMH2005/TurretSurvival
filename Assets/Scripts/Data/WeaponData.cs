using UnityEngine;

public enum AutomaticAttackPattern {
    None,
    NearestBurst,
    NearestChain,
    RadialBurst
}

[CreateAssetMenu(fileName = "WeaponData", menuName = "Scriptable Objects/WeaponData")]
public class WeaponData : ScriptableObject
{
    [Header("Info")]
    public GameObject weaponPrefab;
    public string weaponName;
    public Sprite icon; 
    public Projectile projectilePrefab;
    public float fireRate = 1;
    public float searchRange = 5f;
    public LayerMask enemyLayer;

    [Header("Automatic attack")]
    public AutomaticAttackPattern automaticAttackPattern;
    public float automaticCooldown;
    public int burstCount;
    public float burstInterval;
    public int radialProjectileCount;

    [Header("Manual weapon recovery")]
    public int magazineCapacity = 12;
    public float reloadDuration = 1.5f;
    public float batteryDuration = 3f;
}

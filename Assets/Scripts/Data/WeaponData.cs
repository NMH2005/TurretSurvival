using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Scriptable Objects/WeaponData")]
public class WeaponData : ScriptableObject
{
    public GameObject weaponPrefab;
    public string weaponName;
    public Sprite icon; 
    public Projectile projectilePrefab;
    public float fireRate = 1;
    public float searchRange = 5f;
    public LayerMask enemyLayer;

    [Header("Manual weapon recovery")]
    [Min(1)] public int magazineCapacity = 12;
    [Min(0.05f)] public float reloadDuration = 1.5f;
    [Min(0.05f)] public float batteryDuration = 3f;
}

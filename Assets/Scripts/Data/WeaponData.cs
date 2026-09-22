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
}

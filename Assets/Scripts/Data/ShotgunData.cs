using UnityEngine;

[CreateAssetMenu(fileName = "ShotgunData", menuName = "Scriptable Objects/ShotgunData")]
public class ShotgunData : ScriptableObject
{
    public int pelletCount = 3;
    public float spreadAngle = 30f;
    public Projectile projectilePrefab;
}

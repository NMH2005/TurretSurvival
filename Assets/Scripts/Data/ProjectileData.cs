using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileData", menuName = "Scriptable Objects/ProjectileData")]
public class ProjectileData : ScriptableObject
{
    public float speed = 5f;
    public float lifeTime = 5f;
    public float knockBack = 5f;
    public int damage = 10;
    public int pierceCount = 0;
}

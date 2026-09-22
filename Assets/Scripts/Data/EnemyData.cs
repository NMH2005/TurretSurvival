using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    public float movementSpeed = 3f;
    public int maxHealth = 20;
    public int damage = 10;
    public Vector3 localScale = Vector3.one;
    public float separationRadius = 0.8f;
    public float separationWeight = 1.5f;
    public LayerMask enemyLayer;
    public float attackCoolDown = 1f;
    
}

using UnityEngine;

[CreateAssetMenu(fileName = "LaserData", menuName = "Scriptable Objects/LaserData")]
public class LaserData : ScriptableObject
{
    public float range = 8f;
    public int damagePerTick = 4;
    public float tickRate = 0.1f;
    public LayerMask enemyLayer;
    public bool pierceThrough = true;                                      
}

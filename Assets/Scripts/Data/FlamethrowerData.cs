using UnityEngine;

[CreateAssetMenu(fileName = "FlamethrowerData", menuName = "Scriptable Objects/FlamethrowerData")]
public class FlamethrowerData : ScriptableObject
{
    [Header("DPS")]
    public int damagePerTick = 5;
    public float tickRate = 0.2f;

    [Header("Burn DOT")]
    public int burnDamagePerTick = 3;
    public float burnTickRate = 0.5f;
    public float burnDuration = 2f;

    [Header("Range")]
    public float flameRange = 4f;
    public float flameWidth = 2f;
    public LayerMask enemyLayer;

    [Header("Overheat")]
    public float timeToOverheat = 5f;
    public float cooldownTime = 3f;
    public float overheatJamDuration = 1.5f;
}

using UnityEngine;

[CreateAssetMenu(fileName = "ChainLightningData", menuName = "Scriptable Objects/ChainLightningData")]
public class ChainLightningData : ScriptableObject
{
    public int chainCount = 3;
    public float chainRange = 3f;
    public int damagePerJump = 10;
    public float jumpDelay = 0.08f;
    public LayerMask enemyLayer;
}

using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/ExpData")]
public class ExpData : ScriptableObject
{
    public GameObject expPrefab;
    public int expAmount = 10;
    public float speed = 3f;
}

using UnityEngine;

[CreateAssetMenu(fileName = "EnemyAnimData", menuName = "Scriptable Objects/EnemyAnimData")]
public class EnemyAnimData : ScriptableObject {
    [Header("Run")]
    public Sprite[] runFrames;
    public float runFps = 10f;

    [Header("Die")]
    public Sprite[] dieFrames;
    public float dieFps = 10f;
}

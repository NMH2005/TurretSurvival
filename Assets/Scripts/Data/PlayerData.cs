using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Objects/PlayerData")]
public class PlayerData : ScriptableObject
{
    public int health = 100;
    public int heart = 1;
    public float movementVelocity = 10f;
    public float sidePushForce = 10f;
    public float pickupRange = 2f;
}

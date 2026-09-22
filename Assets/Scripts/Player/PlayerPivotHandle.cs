using UnityEngine;

public class PlayerPivotHandle : MonoBehaviour
{
    [SerializeField] private Transform weaponHoldPoint;
    [SerializeField] private Transform centerPoint;
    [SerializeField] private float radius = 0.5f;
    public PlayerInputHandler InputHandler { get; private set; }

    private void Start()
    {
        InputHandler = GetComponent<PlayerInputHandler>();
    }

    public void RotateTowards(Vector2 targetWorldPosition)
    {
        if (centerPoint == null || weaponHoldPoint == null) return;
        Vector2 direction = targetWorldPosition - (Vector2)centerPoint.position;

        if (direction.sqrMagnitude < 0.01f) return;
        // 2. Tính góc xoay (độ)
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        weaponHoldPoint.position = (Vector2)centerPoint.position + direction.normalized * radius;
        weaponHoldPoint.rotation = Quaternion.Euler(0f, 0f, targetAngle);

    }
}

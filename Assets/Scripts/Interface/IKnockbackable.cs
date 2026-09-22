using UnityEngine;

public interface IKnockbackable {
    void ApplyKnockback(Vector2 dir, float force, float duration = 0.15f);
}
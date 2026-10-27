using UnityEngine;
using System.Collections.Generic;

public class AutomaticAttackController : MonoBehaviour {
    private WeaponBase weapon;

    private float nextAttackTime;
    private float nextShotTime;
    private int remainingShots;

    private readonly List<Collider2D> nearbyEnemies = new(32);
    private WeaponData data => weapon.Data;

    public void Initialize(WeaponBase source)
    {
        weapon = source;
        weapon.Target = null;
        remainingShots = 0;
        nextShotTime = 0f;
        nextAttackTime = Time.time + Mathf.Max(0.05f, data.automaticCooldown);
        enabled = true;
    }

    private void Update()
    {
        if (weapon == null || weapon.IsManual || !weapon.isActiveAndEnabled)
        {
            return;
        }

        if (Time.timeScale <= 0f || Time.deltaTime <= 0f)
            return;

        if (remainingShots > 0)
        {
            HandleBurst();
            return;
        }

        if (Time.time < nextAttackTime)
            return;

        if (TryBeginAttack())
        {
            nextAttackTime =
                Time.time + Mathf.Max(0.05f, data.automaticCooldown);
        }
        else
        {
            nextAttackTime = Time.time + 0.2f;
        }
    }

    private Transform FindNearestEnemy()
    {
        Vector2 origin = weapon.transform.position;
        ContactFilter2D filter = new();
        filter.SetLayerMask(data.enemyLayer);
        filter.useTriggers = true;

        nearbyEnemies.Clear();

        int count = Physics2D.OverlapCircle(origin, data.searchRange, filter, nearbyEnemies);

        Transform nearest = null;
        float nearestDistanceSquared = float.MaxValue;

        for (int i = 0; i < count; i++)
        {
            Collider2D enemy = nearbyEnemies[i];

            if (enemy == null ||
           !enemy.enabled ||
           !enemy.gameObject.activeInHierarchy)
            {
                continue;
            }

            Vector2 offset = (Vector2)enemy.transform.position - origin;

            float distanceSquared = offset.sqrMagnitude;

            if (distanceSquared < nearestDistanceSquared)
            {
                nearestDistanceSquared = distanceSquared;
                nearest = enemy.transform;
            }
        }
        return nearest;
    }

    private bool TryAimTarget()
    {
        Transform target = weapon.Target;
        Collider2D targetCollider = target != null ? target.GetComponent<Collider2D>() : null;

        float rangeSquared = data.searchRange * data.searchRange;
        bool targetInvalid = targetCollider == null || !targetCollider.enabled || !target.gameObject.activeInHierarchy || ((Vector2)target.position - (Vector2)weapon.transform.position).sqrMagnitude > rangeSquared;

        if (targetInvalid) weapon.Target = FindNearestEnemy();
        if (weapon.Target == null) return false;

        Vector2 direction = (Vector2)weapon.Target.position - (Vector2)(weapon.transform.position);

        if (direction.sqrMagnitude > 0.00001f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            weapon.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        return true;
    }

    private void HandleBurst()
    {
        if (Time.time < nextShotTime) return;

        if (!TryAimTarget())
        {
            remainingShots = 0;
            return;
        }

        weapon.Fire();
        remainingShots--;

        nextShotTime = Time.time + Mathf.Max(0.01f, data.burstInterval);
    }

    private bool TryBeginAttack()
    {
        switch (data.automaticAttackPattern)
        {
            case AutomaticAttackPattern.NearestBurst:
                {
                    weapon.Target = FindNearestEnemy();
                    if (weapon.Target == null) return false;

                    remainingShots = Mathf.Max(1, data.burstCount);
                    nextShotTime = Time.time;

                    HandleBurst();
                    return true;
                }
            case AutomaticAttackPattern.NearestChain:
                {
                    weapon.Target = FindNearestEnemy();
                    if (!TryAimTarget()) return false;

                    weapon.Fire();
                    return true;
                }
            case AutomaticAttackPattern.RadialBurst:
                {
                    if (weapon is ShotgunBase shotgun)
                    {
                        shotgun.FireRadial(Mathf.Max(1, data.radialProjectileCount));

                        return true;
                    }
                    enabled = false;
                    return false;
                }
            default:
                return false;
        }
    }
}

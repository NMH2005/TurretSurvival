using UnityEngine;

public class WeaponSearchState : WeaponAutoState {
    private static Collider2D[] enemyColliders = new Collider2D[20];
    public WeaponSearchState(WeaponBase weapon, WeaponStateMachine stateMachine, WeaponData weaponData) : base(weapon, stateMachine, weaponData)
    {
    }

    public override void Enter()
    {
        base.Enter();
        weapon.Target = null;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        Transform nearest = FindNearestEnemy();
        if (nearest != null)
        {
            weapon.Target = nearest;
            weapon.StateMachine.ChangeState(weapon.AttackState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    private Transform FindNearestEnemy()
    {
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(weaponData.enemyLayer);
        filter.useTriggers = true;

        int count = Physics2D.OverlapCircle(
            weapon.transform.position,
            weaponData.searchRange,
            filter,
            enemyColliders
        );

        Transform nearest = null;
        float nearestDist = float.MaxValue;

        for (int i = 0; i < count; i++)
        {
            float dist = Vector2.Distance(weapon.transform.position, enemyColliders[i].transform.position);
            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearest = enemyColliders[i].transform;
            }
        }

        return nearest;
    }
}

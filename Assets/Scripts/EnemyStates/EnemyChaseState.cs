using System;
using UnityEngine;

public class EnemyChaseState : EnemyState {

    private static Collider2D[] neighborColliders = new Collider2D[10];
    public EnemyChaseState(EnemyController enemyController, EnemyStateMachine stateMachine, EnemyData enemyData, Sprite[] animFrames, float animFps) : base(enemyController, stateMachine, enemyData, animFrames, animFps, loopAnim: true)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (enemyController.Target != null)
        {
            core.Movement.Flip(enemyController.Target.position.x - enemyController.transform.position.x);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        if (enemyController.Target == null) return;
        if (core.Combat.IsKnockedBack) return;

        Vector2 toPlayer = ((Vector2)enemyController.Target.position - (Vector2)enemyController.transform.position).normalized;
        Vector2 separation = CalculateSeparation();
        Vector2 finalDir = (toPlayer + separation * enemyData.separationWeight).normalized;
        core.Movement.SetVelocity(finalDir * enemyData.movementSpeed);

    }

    private Vector2 CalculateSeparation()
    {
        Vector2 separationForce = Vector2.zero;

        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(enemyData.enemyLayer);

        int count = Physics2D.OverlapCircle(
            enemyController.transform.position,
            enemyData.separationRadius,
            filter,
            neighborColliders
        );

        for (int i = 0; i < count; i++)
        {
            Collider2D neightbor = neighborColliders[i];

            if (neightbor.gameObject == enemyController.gameObject) continue;

            Vector2 diff = (Vector2)enemyController.transform.position - (Vector2)neightbor.transform.position;
            float distance = diff.magnitude;

            if (distance > 0.001f)
            {
                separationForce += diff.normalized / distance;
            }
        }

        return separationForce;
    }
}
using UnityEngine;

public class EnemyDieState : EnemyState {
    public EnemyDieState(EnemyController enemyController, EnemyStateMachine stateMachine, EnemyData enemyData, Sprite[] animFrames, float animFps)
        : base(enemyController, stateMachine, enemyData, animFrames, animFps, loopAnim: false)
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

        if (core.SpriteAnimator.IsFinished)
        {
            core.Combat.DropExp();
            Spawner.Instance.ReturnEnemyToPool(enemyController);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
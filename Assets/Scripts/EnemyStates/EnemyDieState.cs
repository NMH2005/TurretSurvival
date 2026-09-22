using UnityEngine;

public class EnemyDieState : EnemyState {
    public EnemyDieState(EnemyController enemyController, EnemyStateMachine stateMachine, EnemyData enemyData, string animBoolName) : base(enemyController, stateMachine, enemyData, animBoolName)
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
        AnimatorStateInfo stateInfo =
        enemyController.Anim.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName(animBoolName) &&
        stateInfo.normalizedTime >= 1f)
        {
            animDone = true;
        }

        if (animDone)
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
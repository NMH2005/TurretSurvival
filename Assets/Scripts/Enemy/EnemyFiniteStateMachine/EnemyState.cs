using UnityEngine;

public class EnemyState
{
    protected Core core;
    protected EnemyController enemyController;
    protected EnemyStateMachine stateMachine;
    protected EnemyData enemyData;
    protected string animBoolName;
    protected bool animDone;
    public EnemyState(EnemyController enemyController, EnemyStateMachine stateMachine, EnemyData enemyData,string animBoolName )
    {
        this.enemyController = enemyController;
        this.stateMachine = stateMachine;
        this.enemyData = enemyData;
        this.animBoolName = animBoolName;
        core = enemyController.Core;
    }

    public virtual void Enter()
    {
        DoChecks();
        animDone = false;
        enemyController.Anim.CrossFade(animBoolName, 0.1f);
    }

    public virtual void Exit()
    {
    }

    public virtual void LogicUpdate()
    {

    }

    public virtual void PhysicsUpdate()
    {
        DoChecks();
    }

    public virtual void DoChecks()
    {

    }
}

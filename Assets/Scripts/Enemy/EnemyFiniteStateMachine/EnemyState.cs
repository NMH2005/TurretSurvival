using UnityEngine;

public class EnemyState
{
    protected Core core;
    protected EnemyController enemyController;
    protected EnemyStateMachine stateMachine;
    protected EnemyData enemyData;
    protected Sprite[] animFrames;
    protected float animFps;
    protected bool loopAnim;
    public EnemyState(EnemyController enemyController, EnemyStateMachine stateMachine, EnemyData enemyData, Sprite[] animFrames, float animFps, bool loopAnim = true)
    {
        this.enemyController = enemyController;
        this.stateMachine = stateMachine;
        this.enemyData = enemyData;
        this.animFrames = animFrames;
        this.animFps = animFps;
        this.loopAnim = loopAnim;
        core = enemyController.Core;
    }
    public virtual void Enter()
    {
        DoChecks();
        core.SpriteAnimator.Play(animFrames, animFps, loopAnim);
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

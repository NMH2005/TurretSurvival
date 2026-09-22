using UnityEngine;

public class PlayerMoveState : PlayerGroundedState {
    public PlayerMoveState(PlayerController playerController, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName)
        : base(playerController, stateMachine, playerData, animBoolName) { }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();
        playerController.Anim.SetBool("Move", true);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (input.x == 0 && input.y == 0)
        {
            playerController.StateMachine.ChangeState(playerController.IdleState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        Vector2 moveDirection = input.normalized;
        core.Movement.SetVelocity(moveDirection * playerData.movementVelocity);
    }
}

using UnityEngine;

public class PlayerState {
    protected Core core;
    protected PlayerController playerController;
    protected PlayerStateMachine stateMachine;
    protected PlayerData playerData;
    private string animBoolName;

    protected bool isMoving;

    public PlayerState(PlayerController playerController, PlayerStateMachine stateMachine, PlayerData playerData, string animeBoolName)
    {
        this.playerController = playerController;
        this.stateMachine = stateMachine;
        this.playerData = playerData;
        this.animBoolName = animeBoolName;
        core = playerController.Core;
    }

    public virtual void Enter()
    {
        DoChecks();
    }

    public virtual void Exit() 
    {
    } 
    
    public virtual void LogicUpdate()
    {
        isMoving = playerController.InputHandler.RawMovementInput != Vector2.zero;
    }

    public virtual void PhysicsUpdate()
    {

    }

    public virtual void DoChecks() 
    {
    }
}

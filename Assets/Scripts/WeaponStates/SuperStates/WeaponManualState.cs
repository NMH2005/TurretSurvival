using UnityEngine;

public class WeaponManualState : WeaponState {
    protected bool fireInput;

    public WeaponManualState(WeaponBase weapon, WeaponStateMachine stateMachine, WeaponData weaponData) : base(weapon, stateMachine, weaponData)
    {
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
        fireInput = weapon.InputHandler.fireInput;
        weapon.pivotHandle.RotateTowards(weapon.InputHandler.MouseWorldPosition);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}

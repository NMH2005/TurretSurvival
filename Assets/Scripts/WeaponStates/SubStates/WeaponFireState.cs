using UnityEngine;

public class WeaponFireState : WeaponManualState {
    public WeaponFireState(WeaponBase weapon, WeaponStateMachine stateMachine, WeaponData weaponData) : base(weapon, stateMachine, weaponData)
    {
    }

    public override void Enter()
    {
        base.Enter();
        weapon.StartFire();
    }

    public override void Exit()
    {
        base.Exit();
        weapon.StopFire();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if(!fireInput)
        {
            weapon.StateMachine.ChangeState(weapon.AimState);
            return;
        }

        weapon.StartFire();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}


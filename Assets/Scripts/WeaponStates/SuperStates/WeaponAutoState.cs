using UnityEngine;

public class WeaponAutoState : WeaponState 
{
    public WeaponAutoState(WeaponBase weapon, WeaponStateMachine stateMachine, WeaponData weaponData) : base(weapon, stateMachine, weaponData)
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
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}

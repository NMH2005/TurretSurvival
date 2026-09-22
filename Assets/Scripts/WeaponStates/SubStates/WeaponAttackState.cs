using UnityEngine;

public class WeaponAttackState : WeaponAutoState {
    public WeaponAttackState(WeaponBase weapon, WeaponStateMachine stateMachine, WeaponData weaponData) : base(weapon, stateMachine, weaponData)
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
        if (weapon.Target == null || !weapon.Target.gameObject.activeInHierarchy)
        {
            weapon.StateMachine.ChangeState(weapon.SearchState);
            return;
        }

        float distance = Vector2.Distance(weapon.transform.position, weapon.Target.position);
        if (distance > weaponData.searchRange)
        {
            weapon.StateMachine.ChangeState(weapon.SearchState);
            return;
        }

        weapon.pivotHandle.RotateTowards(weapon.Target.position);
        weapon.StartFire();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}


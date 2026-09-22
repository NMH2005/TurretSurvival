using UnityEngine;

public class WeaponState
{
    protected WeaponBase weapon;
    protected WeaponStateMachine stateMachine;
    protected WeaponData weaponData;

    public WeaponState(WeaponBase weapon, WeaponStateMachine stateMachine, WeaponData weaponData)
    {
        this.weapon = weapon;
        this.stateMachine = stateMachine;
        this.weaponData = weaponData;
    }

    public virtual void Enter()
    {
    }

    public virtual void Exit()
    {

    }

    public virtual void LogicUpdate()
    {

    }

    public virtual void PhysicsUpdate()
    {
    }
}

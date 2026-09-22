using UnityEngine;

public class WeaponStateMachine
{
    public WeaponState CurrentState {  get; private set; }

    public void Initialize(WeaponState StartingState)
    {
        CurrentState = StartingState;
        CurrentState.Enter();
    }

    public void ChangeState(WeaponState NewState)
    {
        CurrentState.Exit();
        CurrentState = NewState;
        CurrentState.Enter();
    }
}

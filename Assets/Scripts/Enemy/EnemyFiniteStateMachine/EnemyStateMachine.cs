using UnityEngine;

public class EnemyStateMachine
{
    public EnemyState CurrentState {  get; private set; }

    public void Initialize(EnemyState StartingState)
    {
        CurrentState = StartingState;
        CurrentState.Enter();
    }

    public void ChangeState(EnemyState NewState)
    {
        CurrentState.Exit();
        CurrentState = NewState;
        CurrentState.Enter();
    }
}

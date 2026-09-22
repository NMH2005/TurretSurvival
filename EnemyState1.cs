using UnityEngine;

public class EnemyState {
    protected EnemyController enemyController;
    protected EnemyStateMachine EnemyStateMachine;
    protected EnemyData enemyData;

    public EnemyState(EnemyController enemyController, EnemyStateMachine enemyStateMachine, EnemyData enemyData)
    {
        this.enemyController = enemyController;
        this.EnemyStateMachine = enemyStateMachine;
        this.enemyData = enemyData;
    }
}

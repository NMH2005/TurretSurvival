using UnityEngine;
public class Despawner : CoreComponent {
    private EnemyController enemyController;
    private float checkTimer;

    protected override void Awake()
    {
        base.Awake();
        enemyController = GetComponentInParent<EnemyController>();
    }

    public override void LogicUpdate()
    {
        CheckDespawnDistance();
    }

    private void CheckDespawnDistance()
    {
        if (enemyController.Target == null || Spawner.Instance == null) return;

        checkTimer += Time.deltaTime;
        if (checkTimer < 0.5f) return;
        checkTimer = 0f;

        float distance = Vector2.Distance(core.transform.parent.position, enemyController.Target.position);
        if (distance > Spawner.Instance.Data.despawnDistance)
        {
            Spawner.Instance.ReturnEnemyToPool(enemyController);
        }
    }
}
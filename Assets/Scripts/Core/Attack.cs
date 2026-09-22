using UnityEngine;

public class Attack : CoreComponent
{
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private LayerMask targetLayer;
    private float lasrAttackTime = -999f;
    protected override void Awake()
    {
        base.Awake();
    }

    public void TryAttack(GameObject target)
    {
        if ((targetLayer.value & (1 << target.layer)) == 0) return;

        if (Time.time - lasrAttackTime < enemyData.attackCoolDown) return;

        var damageable = target.GetComponentInChildren<Idamageable>();
        if(damageable == null) return;

        damageable.TakeDamage(enemyData.damage);
        lasrAttackTime = Time.time;
    }
}

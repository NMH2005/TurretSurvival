using UnityEngine;

public class ChainLightningProjectile : Projectile {
    [SerializeField] private ChainLightningData chainData;
    public ChainLightningData ChainTemplate => chainData;
    private ChainLightningEffect.Snapshot snapshot;

    public override void Initialize(ProjectileData data)
    {
        Initialize(data, chainData);
    }

    public void Initialize(ProjectileData projectile, ChainLightningData chain)
    {
        var settings = new ChainLightningEffect.Snapshot(chain);
        base.Initialize(projectile);
        snapshot = settings;
    }

    protected override void OnHitTarget(GameObject target)
    {
        ChainLightningEffect.Instance?.StartChain(
            target.transform,
            snapshot
        );
    }
}

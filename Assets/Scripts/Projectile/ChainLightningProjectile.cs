using UnityEngine;

public class ChainLightningProjectile : Projectile {
    [SerializeField] private ChainLightningData chainData;

    protected override void OnHitTarget(GameObject target)
    {
        ChainLightningEffect.Instance.StartChain(
            target.transform,
            chainData
        );
    }
}
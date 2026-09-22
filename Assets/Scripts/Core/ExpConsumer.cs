using System;
using UnityEngine;

public class ExpConsumer : CoreComponent {
    [SerializeField] private PlayerData playerData;
    [SerializeField] private LayerMask expLayer;

    private static Collider2D[] expColliders = new Collider2D[30];

    public Vector2 EntityPos => core.transform.parent.position;

    public override void LogicUpdate()
    {
        DetectExpInRange();
    }

    private void DetectExpInRange()
    {
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(expLayer);
        filter.useTriggers = true;

        int count = Physics2D.OverlapCircle(EntityPos, playerData.pickupRange, filter, expColliders);

        for (int i = 0; i < count; i++)
        {
            if(expColliders[i].TryGetComponent<ExpOrb>(out var orb)) {
                orb.StartAttract(this);
            }
        }
    }

    public void AddExp(int amount)
    {
        core.LevelSystem.AddExp(amount);
    }
}

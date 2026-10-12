using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainLightningEffect : MonoBehaviour {
    public readonly struct Snapshot
    {
        public readonly int chainCount;
        public readonly float chainRange;
        public readonly int damagePerJump;
        public readonly float jumpDelay;
        public readonly LayerMask enemyLayer;

        public Snapshot(ChainLightningData data)
        {
            if (data == null) throw new System.ArgumentNullException(nameof(data));
            chainCount = data.chainCount;
            chainRange = data.chainRange;
            damagePerJump = data.damagePerJump;
            jumpDelay = data.jumpDelay;
            enemyLayer = data.enemyLayer;
        }
    }
    public static ChainLightningEffect Instance { get; private set; }

    [SerializeField] private LightningBoltParticle lightning;

    private static Collider2D[] results = new Collider2D[20];

    private void Awake()
    {
        Instance = this;
    }

    public void StartChain(Transform target, ChainLightningData data)
    {
        StartChain(target, new Snapshot(data));
    }

    public void StartChain(Transform target, Snapshot data)
    {
        StartCoroutine(ChainRoutine(target, data));
    }

    private IEnumerator ChainRoutine(
        Transform current,
        Snapshot data)
    {
        HashSet<Transform> hit = new()
        {
            current
        };

        for (int i = 0; i < data.chainCount; i++)
        {
            if (current == null) yield break;
            Transform next = FindNearest(
                current.position,
                data,
                hit
            );

            if (next == null)
                yield break;

            LightningBoltParticle bolt =
                Instantiate(lightning);

            bolt.Draw(
                current.position,
                next.position
            );

            next.GetComponentInChildren<Idamageable>()
                ?.TakeDamage(data.damagePerJump);

            hit.Add(next);
            current = next;

            yield return new WaitForSeconds(data.jumpDelay);
        }
    }

    private Transform FindNearest(
        Vector2 position,
        Snapshot data,
        HashSet<Transform> hit)
    {
        ContactFilter2D filter = new();

        filter.SetLayerMask(data.enemyLayer);
        filter.useTriggers = true;

        int count = Physics2D.OverlapCircle(
            position,
            data.chainRange,
            filter,
            results
        );

        Transform nearest = null;
        float closest = float.MaxValue;

        for (int i = 0; i < count; i++)
        {
            Transform target = results[i].transform;

            if (hit.Contains(target))
                continue;

            float distance =
                Vector2.Distance(position, target.position);

            if (distance < closest)
            {
                closest = distance;
                nearest = target;
            }
        }

        return nearest;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainLightningEffect : MonoBehaviour {
    public static ChainLightningEffect Instance { get; private set; }

    [SerializeField] private LightningBoltParticle lightning;

    private static Collider2D[] results = new Collider2D[20];

    private void Awake()
    {
        Instance = this;
    }

    public void StartChain(Transform target, ChainLightningData data)
    {
        StartCoroutine(ChainRoutine(target, data));
    }

    private IEnumerator ChainRoutine(
        Transform current,
        ChainLightningData data)
    {
        HashSet<Transform> hit = new()
        {
            current
        };

        for (int i = 0; i < data.chainCount; i++)
        {
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
        ChainLightningData data,
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
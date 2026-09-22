using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
    [SerializeField] private ProjectileData projectData;
    private Rigidbody2D rb;
    private int remainingPierce;
    private readonly HashSet<GameObject> alreadyHit = new HashSet<GameObject>();

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        rb.linearVelocity = transform.up * projectData.speed;
        remainingPierce = projectData.pierceCount;
        Destroy(gameObject, projectData.lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (alreadyHit.Contains(collision.gameObject)) return;

        bool hitSomething = false;

        var knockbackable = collision.gameObject.GetComponentInChildren<IKnockbackable>();
        if (knockbackable != null)
        {
            knockbackable.ApplyKnockback(transform.up, projectData.knockBack);
            hitSomething = true;
        }

        var damageable = collision.gameObject.GetComponentInChildren<Idamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(projectData.damage);
            hitSomething = true;
        }

        if (!hitSomething) return;

        alreadyHit.Add(collision.gameObject);
        OnHitTarget(collision.gameObject);

        if (remainingPierce <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            remainingPierce--;
        }
    }

    protected virtual void OnHitTarget(GameObject target) { }
}
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
    [SerializeField] private ProjectileData projectData;
    public ProjectileData TemplateData => projectData;
    private float speed;
    private float lifeTime;
    private float knockBack;
    private int damage;
    private int pierceCount;
    private bool initialized;
    private Rigidbody2D rb;
    private int remainingPierce;
    private readonly HashSet<GameObject> alreadyHit = new HashSet<GameObject>();

    public virtual void Initialize(ProjectileData data)
    {
        if (data == null) throw new System.ArgumentNullException(nameof(data));
        if (initialized) throw new System.InvalidOperationException("Projectile is already initialized.");
        speed = data.speed;
        lifeTime = data.lifeTime;
        knockBack = data.knockBack;
        damage = data.damage;
        pierceCount = data.pierceCount;
        initialized = true;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if (!initialized) Initialize(projectData);
        rb.linearVelocity = transform.up * speed;
        remainingPierce = pierceCount;
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (alreadyHit.Contains(collision.gameObject)) return;

        bool hitSomething = false;

        var knockbackable = collision.gameObject.GetComponentInChildren<IKnockbackable>();
        if (knockbackable != null)
        {
            knockbackable.ApplyKnockback(transform.up, knockBack);
            hitSomething = true;
        }

        var damageable = collision.gameObject.GetComponentInChildren<Idamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
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

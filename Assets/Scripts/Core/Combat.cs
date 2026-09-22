using System;
using UnityEngine;
public class Combat : CoreComponent, Idamageable, IKnockbackable {
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private ExpData expData;
    public Collider2D Collider { get; private set; }
    public int CurrentHealth { get; private set; }
    public bool IsKnockedBack => knockbackTimer > 0;

    public event Action OnDeath;

    private Vector2 knockbackVelocity;
    private float knockbackTimer;

    protected override void Awake()
    {
        base.Awake();
        Collider = GetComponentInParent<Collider2D>();
    }

    public override void PhysicsUpdate()
    {
        if (knockbackTimer > 0)
        {
            knockbackTimer -= Time.fixedDeltaTime;
            core.Movement.SetVelocity(knockbackVelocity);
        }
    }

    public void Initialize()
    {
        CurrentHealth = enemyData.maxHealth;
        Collider.enabled = true;
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        Vector3 popupPosition = transform.position + Vector3.up * 0.8f;
        DamagePopupManager.Instance?.Show(damage, popupPosition, Color.white);
        if (CurrentHealth <= 0)
        {
            Collider.enabled = false;
            OnDeath?.Invoke();
        }
    }

    public void ApplyKnockback(Vector2 dir, float force, float duration = 0.15f)
    {
        knockbackVelocity = dir.normalized * force;
        knockbackTimer = duration;
    }

    public void DropExp()
    {
        Instantiate(expData.expPrefab, core.transform.parent.position, Quaternion.identity);
    }
}
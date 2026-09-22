using UnityEngine;


public class BurnEffect : MonoBehaviour {
    private int damagePerTick;
    private float tickRate;
    private float remainingDuration;
    private float tickTimer;
    private Idamageable damageable;

    public void Apply(int damagePerTick, float tickRate, float duration)
    {
        this.damagePerTick = damagePerTick;
        this.tickRate = tickRate;
        this.remainingDuration = duration;
        this.tickTimer = tickRate;
    }

    public void Refresh(int damagePerTick, float tickRate, float duration)
    {
        this.damagePerTick = damagePerTick;
        this.tickRate = tickRate;
        this.remainingDuration = duration;
    }

    private void Start()
    {
        damageable = GetComponentInChildren<Idamageable>();
    }

    private void Update()
    {
        if (damageable == null || damagePerTick <= 0 || tickRate <= 0)
        {
            Destroy(this);
            return;
        }

        remainingDuration -= Time.deltaTime;
        if (remainingDuration <= 0f)
        {
            Destroy(this);
            return;
        }

        tickTimer -= Time.deltaTime;
        if (tickTimer <= 0f)
        {
            damageable.TakeDamage(damagePerTick);
            tickTimer = tickRate;
        }
    }
}

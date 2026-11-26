using UnityEngine;


public class BurnEffect : MonoBehaviour {
    private int damagePerTick;
    //Dmg/s
    private float tickRate;
    //Burn effect time left
    private float remainingDuration;
    //Time for when burn apply dmg
    private float tickTimer;
    private Idamageable damageable;
    private bool isBurning;
    private BurnVisual burnVisual;
    //Time for when to apply burn visual
    private float visualTimer;
    public void Apply(int damagePerTick, float tickRate, float duration)
    {
        this.damagePerTick = damagePerTick;
        this.tickRate = tickRate;
        this.remainingDuration = duration;
        this.tickTimer = tickRate;

        isBurning = true;
        visualTimer = 0f;
        burnVisual.PlayLoop();
    }

    public void Refresh(int damagePerTick, float tickRate, float duration)
    {
        this.damagePerTick = damagePerTick;
        this.tickRate = tickRate;
        this.remainingDuration = duration;

        isBurning = true;
        burnVisual.PlayLoop();
    }

    private void Awake()
    {
        burnVisual = GetComponentInChildren<BurnVisual>();
        damageable = GetComponentInChildren<Idamageable>();
    }

    private void Update()
    {
        if (!isBurning) return;
        remainingDuration -= Time.deltaTime;

        if (remainingDuration <= 0f)
        {
            StopBurn();
            return;
        }

        burnVisual.TickLoop(Time.deltaTime);

        tickTimer -= Time.deltaTime;
        if (tickTimer > 0f) return;

        burnVisual.Flash();
        damageable.TakeDamage(damagePerTick);
        tickTimer += tickRate;
    }

    private void StopBurn()
    {
        isBurning = false;
        burnVisual.Stop();
        Destroy(this);
    }
}

using UnityEngine;

public class Health : CoreComponent, Idamageable
{
    [SerializeField] private PlayerData playerData;
    public int CurrentHealth { get; private set; }
    public int CurrentHeart {  get; private set; }
    public int MaxHealth => playerData.health;
    public bool IsDead => CurrentHealth <= 0 && CurrentHeart <= 0;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        CurrentHealth = playerData.health;
        CurrentHeart = playerData.heart;
        EventsManager.RaiseHealthChanged(CurrentHealth, CurrentHeart);
    }

    public void TakeDamage(int damage)
    {
        if (IsDead) return;

        CurrentHealth -= damage;
        while (CurrentHealth <= 0 && CurrentHeart > 0)
        {
            CurrentHeart--;

            if (CurrentHeart > 0)
            {
                CurrentHealth = MaxHealth;
            }
        }
        EventsManager.RaiseHealthChanged(CurrentHealth, CurrentHeart);

        if(IsDead)
        {
            EventsManager.RaisePlayerDeath();
        }
    }


}

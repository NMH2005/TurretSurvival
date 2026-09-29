using UnityEngine;

public class Core : MonoBehaviour {
    private Movement movement;
    private Combat combat;
    private ExpConsumer expConsumer;
    private LevelSystem levelSystem;
    private Health health;
    private Attack attack;
    private WeaponSystem weaponSystem;
    private SpriteAnimator spriteAnimator;

    public SpriteAnimator SpriteAnimator
    {
        get
        {
            if(spriteAnimator == null) spriteAnimator = GetComponentInChildren<SpriteAnimator>();
            return spriteAnimator;
        }
    }
    public WeaponSystem WeaponSystem
    {
        get
        {
            if (weaponSystem == null) weaponSystem = GetComponentInChildren<WeaponSystem>();
            return weaponSystem;
        }
    }
    public Movement Movement
    {
        get
        {
            if (movement == null) movement = GetComponentInChildren<Movement>();
            return movement;
        }
    }

    public Combat Combat
    {
        get
        {
            if (combat == null) combat = GetComponentInChildren<Combat>();
            return combat;
        }
    }

    public ExpConsumer ExpConsumer
    {
        get
        {
            if (expConsumer == null) expConsumer = GetComponentInChildren<ExpConsumer>();
            return expConsumer;
        }
    }

    public LevelSystem LevelSystem
    {
        get
        {
            if (levelSystem == null) levelSystem = GetComponentInChildren<LevelSystem>();
            return levelSystem;
        }
    }

    public Health Health
    {
        get
        {
            if (health == null) health = GetComponentInChildren<Health>();
            return health;
        }
    }

    public Attack Attack
    {
        get
        {
            if(attack == null) attack = GetComponentInChildren<Attack>();
            return attack;
        }
    }
    private CoreComponent[] allCoreComponents;

    private void Awake()
    {
        allCoreComponents = GetComponentsInChildren<CoreComponent>();
    }

    public void LogicUpdate()
    {
        foreach (var component in allCoreComponents)
        {
            component.LogicUpdate();
        }
    }

    public void PhysicsUpdate()
    {
        foreach (var component in allCoreComponents)
        {
            component.PhysicsUpdate();
        }
    }
}
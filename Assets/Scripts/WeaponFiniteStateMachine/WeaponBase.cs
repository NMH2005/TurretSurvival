using UnityEngine;

public class WeaponBase : MonoBehaviour {

    [SerializeField] private WeaponData weaponData;
    [SerializeField] private Transform firePoint;
    public PlayerPivotHandle pivotHandle { get; private set; }
    public PlayerInputHandler InputHandler { get; private set; }
    public WeaponStateMachine StateMachine { get; private set; }
    public WeaponAimState AimState { get; private set; }
    public WeaponFireState FireState { get; private set; }
    public WeaponSearchState SearchState { get; private set; }
    public WeaponAttackState AttackState { get; private set; }
    public float LastFireTime { get; set; }
    public Transform FirePoint => firePoint;
    public WeaponData Data => weaponData;

    public Transform Target { get; set; }

    public bool IsManual { get; private set; } = true;
    public void SetControlMode(bool isManual)
    {
        IsManual = isManual;
    }
    private void Start()
    {
        pivotHandle = GetComponentInParent<PlayerPivotHandle>();
        InputHandler = GetComponentInParent<PlayerInputHandler>();

        StateMachine = new WeaponStateMachine();
        AimState = new WeaponAimState(this, StateMachine, weaponData);
        FireState = new WeaponFireState(this, StateMachine, weaponData);
        SearchState = new WeaponSearchState(this, StateMachine, weaponData);
        AttackState = new WeaponAttackState(this, StateMachine, weaponData);

        StateMachine.Initialize(IsManual ? (WeaponState)AimState : SearchState);
    }
    protected virtual void Update()
    {
        StateMachine.CurrentState.LogicUpdate();
    }

    private void FixedUpdate()
    {
        StateMachine.CurrentState.PhysicsUpdate();
    }


    public virtual void Fire()
    {
        if (firePoint == null || weaponData.projectilePrefab == null) return;
        Instantiate(weaponData.projectilePrefab, firePoint.position, firePoint.rotation);
        LastFireTime = Time.time;
    }

    public virtual void StartFire()
    {
        if (Time.time >= LastFireTime + weaponData.fireRate)
        {
            Fire();
        }
    }

    public virtual void StopFire()
    {
    }
}

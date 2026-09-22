using UnityEngine;
using UnityEngine.Pool;

public class EnemyController : MonoBehaviour {
    [SerializeField] private EnemyData enemyData;
    #region components
    public Core Core { get; private set; }
    public Animator Anim { get; private set; }
    #endregion

    #region state variables
    public EnemyStateMachine StateMachine { get; private set; }
    public EnemyChaseState ChaseState { get; private set; }
    public EnemyDieState DieState { get; private set; }
    #endregion
    public Transform Target { get; private set; }

    private IObjectPool<EnemyController> enemyPool;

    private void Awake()
    {
        Core = GetComponentInChildren<Core>();
        Anim = GetComponent<Animator>();
        StateMachine = new EnemyStateMachine();
        ChaseState = new EnemyChaseState(this, StateMachine, enemyData, "EnemyRun");
        DieState = new EnemyDieState(this, StateMachine, enemyData, "EnemyDie");

        Core.Combat.OnDeath += HandleDeath;
    }

    private void Start()
    {
        FindTarget();
        Core.Combat.Initialize();
        StateMachine.Initialize(ChaseState);
    }

    private void Update()
    {
        StateMachine.CurrentState.LogicUpdate();
        Core.LogicUpdate();
    }

    private void FixedUpdate()
    {
        StateMachine.CurrentState.PhysicsUpdate();
        Core.PhysicsUpdate();
    }

    private void FindTarget()
    {
        PlayerController player = FindAnyObjectByType<PlayerController>();
        if (player != null)
        {
            Target = player.transform;
        }
    }

    public void ResetEnemy(Vector2 spawnPosition)
    {
        transform.position = spawnPosition;
        Core.Movement.SetVelocityZero();

        if (Target == null)
        {
            FindTarget();
        }

        Core.Combat.Initialize();
        StateMachine.Initialize(ChaseState);
    }

    private void HandleDeath()
    {
        StateMachine.ChangeState(DieState);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Core.Attack.TryAttack(collision.gameObject);
    }
}
using UnityEngine;

public class PlayerController : MonoBehaviour {
    [SerializeField] private PlayerData playerData;
    #region components
    public Core Core { get; private set; }
    public PlayerInputHandler InputHandler { get; private set; }
    public Rigidbody2D RB { get; private set; }
public Animator Anim { get; private set; }
    #endregion
    #region state variables
    public PlayerStateMachine StateMachine { get; private set; }
    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    #endregion
    
    private void Awake()
    {
        Core = GetComponentInChildren<Core>();
        InputHandler = GetComponent<PlayerInputHandler>();
        RB = GetComponent<Rigidbody2D>();
        Anim = GetComponent<Animator>();
        StateMachine = new PlayerStateMachine();
        IdleState = new PlayerIdleState(this, StateMachine, playerData, "Idle");
        MoveState = new PlayerMoveState(this, StateMachine, playerData, "Move");
    }

    private void Start()
    {
        StateMachine.Initialize(IdleState);
    }

    private void Update()
    {
        StateMachine.CurrentState.LogicUpdate();
        Core.LogicUpdate();
    }

    private void FixedUpdate()
    {
        StateMachine.CurrentState.PhysicsUpdate();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        var knockbackable = collision.gameObject.GetComponentInChildren<IKnockbackable>();
        if (knockbackable == null) return;

        Vector2 playerMoveDir = RB.linearVelocity.normalized;
        if (playerMoveDir.sqrMagnitude <= 0.01f) return;

        Vector2 toEnemy = ((Vector2)collision.transform.position - (Vector2)transform.position).normalized;
        Vector2 sideDirection = Vector2.Perpendicular(playerMoveDir);
        float sideDot = Vector2.Dot(toEnemy, sideDirection);
        float sideSign = sideDot >= 0 ? 1 : -1;

        knockbackable.ApplyKnockback(sideDirection * sideSign, playerData.sidePushForce);
    }

}

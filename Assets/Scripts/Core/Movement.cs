using UnityEngine;

public class Movement : CoreComponent {
    public Rigidbody2D RB { get; private set; }

    private Transform entityTransform;
    private Vector3 baseScale;

    protected override void Awake()
    {
        base.Awake();
        RB = GetComponentInParent<Rigidbody2D>();

        entityTransform = core.transform.parent;
        baseScale = entityTransform.localScale;
    }

    public void SetVelocity(Vector2 velocity)
    {
        RB.linearVelocity = velocity;
    }

    public void SetVelocityZero()
    {
        RB.linearVelocity = Vector2.zero;
    }

    public void SetVelocityX(float xVelocity)
    {
        RB.linearVelocity = new Vector2(xVelocity, RB.linearVelocity.y);
    }

    public void SetVelocityY(float yVelocity)
    {
        RB.linearVelocity = new Vector2(RB.linearVelocity.x, yVelocity);
    }

    public void Flip(float xDirection)
    {
        if (xDirection > 0)
        {
            entityTransform.localScale = new Vector3(Mathf.Abs(baseScale.x), baseScale.y, baseScale.z);
        }
        else if (xDirection < 0)
        {
            entityTransform.localScale = new Vector3(-Mathf.Abs(baseScale.x), baseScale.y, baseScale.z);
        }
    }
}
using UnityEngine;

public class SpriteAnimator : CoreComponent
{
    private SpriteRenderer spriteRenderer;
    private Sprite[] currentFrames;
    private float frameDuration;
    private float frameTimer;
    private int currentFrameIndex;
    private bool loop;
    public bool IsFinished { get; private set; }
    protected override void Awake()
    {
        base.Awake();
        spriteRenderer = GetComponentInParent<SpriteRenderer>();
    }

    public override void LogicUpdate()
    {
        if (currentFrames == null || currentFrames.Length == 0 || IsFinished) return;

        frameTimer += Time.deltaTime;
        if (frameTimer < frameDuration) return;
        frameTimer = 0f;

        currentFrameIndex++;

        if (currentFrameIndex >= currentFrames.Length)
        {
            if (loop)
            {
                currentFrameIndex = 0;
            }
            else
            {
                currentFrameIndex = currentFrames.Length - 1;
                IsFinished = true;
            }
        }

        spriteRenderer.sprite = currentFrames[currentFrameIndex];
    }

    public void Play(Sprite[] frames, float fps, bool loop = true)
    {
        if (currentFrames == frames) return;

        currentFrames = frames;
        frameDuration = fps > 0f ? 1f / fps : 0.1f;
        this.loop = loop;
        currentFrameIndex = 0;
        frameTimer = 0f;
        IsFinished = false;

        if (frames != null && frames.Length > 0)
        {
            spriteRenderer.sprite = frames[0];
        }
    }
}

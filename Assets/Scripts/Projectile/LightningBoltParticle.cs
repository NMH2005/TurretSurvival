using System;
using UnityEngine;

public class LightningBoltParticle : MonoBehaviour {
    private const int FrameCount = 28;
    private const int Columns = 5;
    private const int Rows = 6;
    private const float ArcWorldLength = 0.66f;
    private static readonly Vector2 Pivot = new Vector2(76f / 144f, 0.5f);

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Texture2D texture;
    [SerializeField, Min(1f)] private float framesPerSecond = 30f;
    [SerializeField, Min(0.1f)] private float lifeTime = 0.95f;

    private Sprite[] frames;
    private float frameTimer;
    private int frameIndex;

    private void Awake()
    {
        frames = CreateFrames(texture);
        if (spriteRenderer != null) spriteRenderer.enabled = false;
    }

    public void Draw(Vector2 start, Vector2 end)
    {
        Vector2 direction = end - start;
        transform.position = end;
        transform.rotation = Quaternion.Euler(
            0f,
            0f,
            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        transform.localScale = new Vector3(
            Mathf.Max(0.05f, direction.magnitude / ArcWorldLength),
            1f,
            1f);

        frameIndex = 0;
        frameTimer = 0f;
        if (frames.Length > 0 && spriteRenderer != null)
        {
            spriteRenderer.sprite = frames[0];
            spriteRenderer.enabled = true;
        }
    }

    private void Update()
    {
        if (frames == null || frames.Length == 0 || spriteRenderer == null) return;

        frameTimer += Time.deltaTime;
        float frameDuration = 1f / framesPerSecond;
        while (frameTimer >= frameDuration)
        {
            frameTimer -= frameDuration;
            frameIndex++;
            if (frameIndex >= frames.Length)
            {
                Destroy(gameObject);
                return;
            }
            spriteRenderer.sprite = frames[frameIndex];
        }
    }

    private static Sprite[] CreateFrames(Texture2D sheet)
    {
        if (sheet == null) return Array.Empty<Sprite>();

        int frameWidth = sheet.width / Columns;
        int frameHeight = sheet.height / Rows;
        var result = new Sprite[FrameCount];
        for (int i = 0; i < result.Length; i++)
        {
            int column = i % Columns;
            int row = i / Columns;
            result[i] = Sprite.Create(
                sheet,
                new Rect(
                    column * frameWidth,
                    sheet.height - (row + 1) * frameHeight,
                    frameWidth,
                    frameHeight),
                Pivot,
                100f,
                0,
                SpriteMeshType.FullRect);
        }
        return result;
    }

    private void OnDestroy()
    {
        if (frames == null) return;
        foreach (Sprite frame in frames)
        {
            if (frame != null) Destroy(frame);
        }
    }
}

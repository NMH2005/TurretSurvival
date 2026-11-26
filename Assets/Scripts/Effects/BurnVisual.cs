using System;
using System.Collections.Generic;
using UnityEngine;

public class BurnVisual : MonoBehaviour {
    private static readonly Vector2 Pivot = new Vector2(0.5f, 0.4f);
    [SerializeField] private SpriteRenderer fireRenderer;
    [SerializeField] private Texture2D sheets;
    [SerializeField] private float framesPerSecond = 30f;
    [SerializeField] private int columns = 7;
    [SerializeField] private int rows = 6;
    [SerializeField] private int frameCount = 36;
    [SerializeField] private Color flashWhite = Color.white;
    [SerializeField] private Color flashOrange = new Color(1f, 0.45f, 0.1f, 1f);
    [SerializeField] private float flashDuration = 0.2f;

    private readonly List<SpriteRenderer> bodyRenderers = new();
    private readonly List<Color> originalColors = new();

    private float flashTimer = -1f;
    private Sprite[] frames;
    private int frameIndex;
    private float frameTimer;
    private bool isPlaying;
    private bool isFlashing;
    private float flashElapsed;
    private bool isReady;

    private void Awake()
    {
        if (fireRenderer == null || sheets == null)
            return;

        frames = CreateFrame(sheets);
        fireRenderer.enabled = false;
        isReady = true;
    }
    public void PlayLoop()
    {
        if (!isReady) return;

        CacheBodyRenderers();

        if (!isPlaying)
        {
            isPlaying = true;
            frameIndex = 0;
            frameTimer = 0f;
        }

        if (fireRenderer != null)
            fireRenderer.enabled = true;
    }

    private void CacheBodyRenderers()
    {
        if (bodyRenderers.Count > 0) return;
        var found = new List<SpriteRenderer>();
        GetComponentsInParent(true, found);

        foreach (SpriteRenderer renderer in found)
        {
            if (renderer == null || renderer == fireRenderer) continue;
            bodyRenderers.Add(renderer);
            originalColors.Add(renderer.color);
        }
    }

    public void Stop()
    {
        if (!isReady) return;

        isPlaying = false;
        isFlashing = false;
        frameIndex = 0;
        frameTimer = 0f;
        flashElapsed = 0f;

        fireRenderer.enabled = false;
        RestoreBodyColors();
    }

    public void TickLoop(float deltaTime)
    {
        if (!isPlaying) return;

        frameTimer += deltaTime;
        float frameDuration = 1f / framesPerSecond;

        while (frameTimer >= frameDuration)
        {
            frameTimer -= frameDuration;
            frameIndex = (frameIndex + 1) % frames.Length;
            fireRenderer.sprite = frames[frameIndex];
        }

        TickFlash(deltaTime);
    }

    public void Flash()
    {
        if (!isReady || !isPlaying) return;

        CacheBodyRenderers();
        isFlashing = true;
        flashElapsed = 0f;

        ApplyFlashFrame();
    }
    private void TickFlash(float visualTimer)
    {
        if (!isFlashing) return;
        flashElapsed += visualTimer;
        if (flashElapsed > flashDuration)
        {
            isFlashing = false;
            RestoreBodyColors();
            return;
        }
        ApplyFlashFrame();
    }

    private void RestoreBodyColors()
    {
        for (int i = 0; i < bodyRenderers.Count; i++)
        {
            bodyRenderers[i].color = originalColors[i];
        }
    }

    private void ApplyFlashFrame()
    {
        for (int i = 0; i < bodyRenderers.Count; i++)
        {
            SpriteRenderer renderer = bodyRenderers[i];
            if (renderer == null) return;

            Color original = originalColors[i];

            if (flashElapsed < 1f / 30f)
            {
                Color white = Color.white;
                white.a = original.a;
                renderer.color = white;
                continue;
            }

            float u = (flashElapsed - 1f / 30f) /
                      (0.2f - 1f / 30f);
            float strength = (1f - Mathf.Clamp01(u)) * 0.55f;

            Color orange = new Color(1f, 0.45f, 0.1f, original.a);
            renderer.color = Color.Lerp(original, orange, strength);
        }
    }

    private Sprite[] CreateFrame(Texture2D texture)
    {
        int framePixelWidth = texture.width / columns;
        int framePixelHeight = texture.height / rows;

        var result = new Sprite[frameCount];

        for (int i = 0; i < frameCount; i++)
        {
            int column = i % columns;
            int row = i / columns;


            var rect = new Rect(
                column * framePixelWidth,
                texture.height - (row + 1) * framePixelHeight,
                framePixelWidth,
                framePixelHeight);

            result[i] = Sprite.Create(texture, rect, Pivot, 32f, 0, SpriteMeshType.FullRect);
        }

        return result;
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

public class FlamethrowerWeapon : MonoBehaviour {
    private const int FlameColumns = 8;
    private const int FlameRows = 12;
    private const int FlameFrameCount = 91;
    private const int FlamePixelsPerUnit = 100;
    private static readonly Vector2 FlamePivot = new Vector2(30f / 184f, 44f / 104f);
    private const int StartFirstFrame = 0;
    private const int StartLastFrame = 24;
    private const int LoopFirstFrame = 25;
    private const int LoopLastFrame = 82;


    [SerializeField] private FlamethrowerData flameData;
    private FlamethrowerData templateData;
    public FlamethrowerData TemplateData => templateData != null ? templateData : flameData;

    public void Initialize(FlamethrowerData data)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));
        if (templateData == null) templateData = flameData;
        flameData = data;
    }
    [SerializeField] private SpriteRenderer flameRenderer;
    [SerializeField] private Texture2D flameTexture;
    [SerializeField, Min(1f)] private float flameFps = 30f;
    [SerializeField, Min(0)] private int flameStartFrame = 21;

    private Collider2D flameCollider;
    private bool isFiring;
    private float tickTimer;
    private Sprite[] flameFrames;
    private int flameFrameIndex;
    private float flameFrameTimer;
    private bool flameStartFinished;

    //Scale-based range
    private Vector3 initialScale;
    private float baseRange;
    //Overheat
    private float currentHeat = 0f;
    private bool isOverheated = false;
    private float jamTimer = 0f;
    private bool heatEnabled = true;

    private List<Idamageable> enemiesInRange = new List<Idamageable>();
    private Dictionary<Idamageable, GameObject> enemyGameObjects = new Dictionary<Idamageable, GameObject>();

    public bool CanFire => !heatEnabled || !isOverheated;
    public float HeatNormalized => currentHeat;
    public bool IsOverheated => isOverheated;

    public void SetHeatEnabled(bool enabled)
    {
        heatEnabled = enabled;
        if (!enabled)
        {
            currentHeat = 0f;
            isOverheated = false;
            jamTimer = 0f;
        }
    }
    private void Awake()
    {
        flameCollider = GetComponent<Collider2D>();

        if (flameCollider != null)
        {
            flameCollider.isTrigger = true;
            flameCollider.enabled = false;
        }
        initialScale = transform.localScale;
        baseRange = flameData.flameRange;
        flameFrames = CreateFlameFrames(flameTexture);
        if (flameRenderer != null) flameRenderer.enabled = false;

        UpdateRangeScale();
    }

    private void UpdateRangeScale()
    {
        float multiplier = flameData.flameRange / baseRange;
        transform.localScale = initialScale * multiplier;
    }

    public void StartFlame()
    {
        if (!CanFire || isFiring || Time.timeScale <= 0f) return;

        isFiring = true;
        tickTimer = 0f;

        UpdateRangeScale();

        if (flameCollider != null)
            flameCollider.enabled = true;

        PlayFlameVisual();
    }

    public void StopFlame()
    {
        if (!isFiring) return;
        isFiring = false;

        if (flameCollider != null)
            flameCollider.enabled = false;

        if (flameRenderer != null)
            flameRenderer.enabled = false;

        foreach (var enemyObj in enemyGameObjects.Values)
        {
            if (enemyObj != null && enemyObj.activeInHierarchy)
            {
                ApplyBurn(enemyObj);
            }
        }

        enemiesInRange.Clear();
        enemyGameObjects.Clear();
    }

    private void Update()
    {
        if (Time.timeScale <= 0f || Time.deltaTime <= 0f) return;
        HandleHeatLogic();
        if (!isFiring) return;

        AdvanceFlameVisual(Time.deltaTime);
        tickTimer -= Time.deltaTime;
        if (tickTimer <= 0f)
        {
            tickTimer = flameData.tickRate;
            DealDamageToEnemiesInRange();
        }
    }

    private Sprite[] CreateFlameFrames(Texture2D texture)
    {
        if (texture == null) return Array.Empty<Sprite>();

        int frameWidth = texture.width / FlameColumns;
        int frameHeight = texture.height / FlameRows;
        var frames = new Sprite[FlameFrameCount];
        for (int i = 0; i < frames.Length; i++)
        {
            int column = i % FlameColumns;
            int row = i / FlameColumns;
            var rect = new Rect(
                column * frameWidth,
                texture.height - (row + 1) * frameHeight,
                frameWidth,
                frameHeight);
            frames[i] = Sprite.Create(
                texture,
                rect,
                FlamePivot,
                FlamePixelsPerUnit,
                0,
                SpriteMeshType.FullRect);
        }
        return frames;
    }

    private void PlayFlameVisual()
    {
        if (flameRenderer == null || flameFrames == null || flameFrames.Length == 0) return;

        flameFrameIndex = StartFirstFrame;
        flameFrameTimer = 0f;
        flameStartFinished = false;
        flameRenderer.sprite = flameFrames[flameFrameIndex];
        flameRenderer.enabled = true;
    }

    private void AdvanceFlameVisual(float deltaTime)
    {
        if (flameRenderer == null || flameFrames == null || flameFrames.Length == 0) return;

        flameFrameTimer += deltaTime;
        float frameDuration = 1f / flameFps;
        if (flameFrameTimer < frameDuration) return;

        int framesToAdvance = Mathf.FloorToInt(flameFrameTimer / frameDuration);
        flameFrameTimer %= frameDuration;
        for (int i = 0; i < framesToAdvance; i++)
            AdvanceOneFlameFrame();
    }

    private void AdvanceOneFlameFrame()
    {
        if (!flameStartFinished)
        {
            if (flameFrameIndex >= StartLastFrame)
            {
                flameStartFinished = true;
                flameFrameIndex = LoopFirstFrame;
            }
            else
            {
                flameFrameIndex++;
            }
        }
        else
        {
            flameFrameIndex++;

            if (flameFrameIndex > LoopLastFrame)
                flameFrameIndex = LoopFirstFrame;
        }

        flameRenderer.sprite = flameFrames[flameFrameIndex];
    }

    private void OnDestroy()
    {
        if (flameFrames == null) return;
        foreach (Sprite frame in flameFrames)
        {
            if (frame != null) Destroy(frame);
        }
    }

    private void HandleHeatLogic()
    {
        if (!heatEnabled) return;
        if (isFiring)
        {
            currentHeat += Time.deltaTime / Mathf.Max(0.05f, flameData.timeToOverheat);
            if (currentHeat >= 1f)
            {
                currentHeat = 1f;
                isOverheated = true;
                jamTimer = flameData.overheatJamDuration;
                StopFlame();
            }
            return;
        }

        float coolingTime = Time.deltaTime;
        if (isOverheated && jamTimer > 0f)
        {
            float jamTime = Mathf.Min(jamTimer, coolingTime);
            jamTimer -= jamTime;
            coolingTime -= jamTime;
        }
        currentHeat = Mathf.Max(0f, currentHeat - coolingTime / Mathf.Max(0.05f, flameData.cooldownTime));
        if (currentHeat <= 0f) isOverheated = false;
    }

    private void DealDamageToEnemiesInRange()
    {
        for (int i = enemiesInRange.Count - 1; i >= 0; i--)
        {
            var damageable = enemiesInRange[i];

            if (damageable == null || !enemyGameObjects.TryGetValue(damageable, out GameObject enemyObj) || enemyObj == null || !enemyObj.activeInHierarchy)
            {
                enemiesInRange.RemoveAt(i);
                continue;
            }

            damageable.TakeDamage(flameData.damagePerTick);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isFiring || Time.timeScale <= 0f) return;
        if (((1 << collision.gameObject.layer) & flameData.enemyLayer) == 0) return;

        var damageable = collision.GetComponentInChildren<Idamageable>();
        if (damageable != null && !enemiesInRange.Contains(damageable))
        {
            enemiesInRange.Add(damageable);
            enemyGameObjects[damageable] = collision.gameObject;

            damageable.TakeDamage(flameData.damagePerTick);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var damageable = collision.GetComponentInChildren<Idamageable>();
        if (damageable != null)
        {
            enemiesInRange.Remove(damageable);
            if (enemyGameObjects.TryGetValue(damageable, out GameObject enemyObj))
            {
                if (enemyObj != null && enemyObj.activeInHierarchy)
                {
                    ApplyBurn(enemyObj);
                }
                enemyGameObjects.Remove(damageable);
            }
        }
    }

    private void ApplyBurn(GameObject enemy)
    {
        if (flameData.burnDuration <= 0f) return;

        BurnEffect burn = enemy.GetComponent<BurnEffect>();
        if (burn == null)
        {
            burn = enemy.AddComponent<BurnEffect>();
            burn.Apply(flameData.burnDamagePerTick, flameData.burnTickRate, flameData.burnDuration);
        }
        else
        {
            burn.Refresh(flameData.burnDamagePerTick, flameData.burnTickRate, flameData.burnDuration);
        }
    }
}

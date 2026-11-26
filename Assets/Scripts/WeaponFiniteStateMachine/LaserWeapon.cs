using UnityEngine;

public class LaserWeapon : MonoBehaviour {
    [SerializeField] private LaserData laserData;
    [SerializeField] private Transform firePoint;

    private LaserData templateData;
    public LaserData TemplateData => templateData != null ? templateData : laserData;

    public void Initialize(LaserData data)
    {
        if (data == null) throw new System.ArgumentNullException(nameof(data));
        if (templateData == null) templateData = laserData;
        laserData = data;
    }

    private const int BeamFrameCount = 36;
    private const int BeamColumns = 4;
    private const int BeamRows = 9;
    private const float BeamPixelsPerUnit = 100f;
    private const float BeamArtLength = 1.68f;
    private const float BeamSourceOffset = 0.15f;
    private static readonly Vector2 BeamPivot =
        new Vector2(26f / 264f, 16f / 100f);

    [SerializeField] private SpriteRenderer beamRenderer;
    [SerializeField] private Texture2D beamSheet;
    [SerializeField, Min(1f)] private float beamFps = 30f;

    private Sprite[] beamFrames;
    private int beamFrameIndex;
    private float beamFrameTimer;

    private bool isFiring;
    public bool IsFiring => isFiring;

    private float tickTimer;

    private static RaycastHit2D[] hitBuffer = new RaycastHit2D[10];

    private void Awake()
    {
        beamFrames = CreateBeamFrames(beamSheet);

        if (beamRenderer != null)
        {
            beamRenderer.transform.localPosition = Vector3.zero;
            beamRenderer.transform.localRotation =
                Quaternion.Euler(0f, 0f, 90f);

            beamRenderer.enabled = false;
        }
    }

    private Sprite[] CreateBeamFrames(Texture2D texture)
    {
        if (texture == null)
            return System.Array.Empty<Sprite>();

        int frameWidth = texture.width / BeamColumns;
        int frameHeight = texture.height / BeamRows;
        var frames = new Sprite[BeamFrameCount];

        for (int i = 0; i < frames.Length; i++)
        {
            int column = i % BeamColumns;
            int row = i / BeamColumns;

            frames[i] = Sprite.Create(texture, new Rect(column * frameWidth, texture.height - (row + 1) * frameHeight, frameWidth, frameHeight), BeamPivot, BeamPixelsPerUnit, 0, SpriteMeshType.FullRect);
        }

        return frames;
    }

    public void StartLaser()
    {
        if (isFiring) return;

        isFiring = true;
        tickTimer = 0f;
        beamFrameIndex = 0;
        beamFrameTimer = 0f;

        if (beamRenderer != null && beamFrames.Length > 0)
        {
            beamRenderer.sprite = beamFrames[0];
            beamRenderer.enabled = true;
        }
    }

    public void StopLaser()
    {
        if (!isFiring)
            return;
        isFiring = false;

        if (beamRenderer != null)
        {
            beamRenderer.enabled = false;
        }
    }

    private void Update()
    {
        if (!isFiring || Time.timeScale <= 0f || Time.deltaTime <= 0f) return;

        Vector2 origin = firePoint.position;
        Vector2 direction = firePoint.up;

        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(laserData.enemyLayer);
        filter.useTriggers = true;

        int count = Physics2D.Raycast(origin, direction, filter, hitBuffer, laserData.range);

        float hitDistance = laserData.range;

        tickTimer -= Time.deltaTime;
        bool shouldTick = tickTimer <= 0f;
        if (shouldTick) tickTimer = laserData.tickRate;

        if (count > 0)
        {
            hitDistance = hitBuffer[0].distance;

            if (shouldTick)
            {
                int hitsToApply = laserData.pierceThrough ? count : 1;
                for (int i = 0; i < hitsToApply; i++)
                {
                    var damageable = hitBuffer[i].collider.GetComponentInChildren<Idamageable>();
                    damageable?.TakeDamage(laserData.damagePerTick);
                }
            }
        }

        if (beamRenderer != null && beamFrames.Length > 0)
        {

            float parentScale = firePoint.TransformVector(Vector3.up).magnitude;

            if (parentScale <= 0.0001f) parentScale = 1f;

            float beamScaleX = Mathf.Max( 0.01f, hitDistance / (BeamArtLength * parentScale)); beamRenderer.transform.localScale = new Vector3(beamScaleX, 1f, 1f);
            beamRenderer.transform.localPosition = Vector3.down * (BeamSourceOffset * beamScaleX);
            beamFrameTimer += Time.deltaTime;
            float frameDuration = 1f / beamFps;
            if (beamFrameTimer >= frameDuration)
            {
                int framesToAdvance = Mathf.FloorToInt(beamFrameTimer / frameDuration);
                beamFrameTimer %= frameDuration;
                beamFrameIndex = (beamFrameIndex + framesToAdvance) % beamFrames.Length;
                beamRenderer.sprite = beamFrames[beamFrameIndex];
            }
        }
    }

    private void OnDestroy()
    {
        if (beamFrames == null) return;

        foreach (Sprite frame in beamFrames)
        {
            if (frame != null)
                Destroy(frame);
        }
    }
}

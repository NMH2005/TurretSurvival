using System.Collections.Generic;
using UnityEngine;

public class ElectrocutedEffect : MonoBehaviour {
    private static readonly Color ElectricBlue = new Color(0.2f, 0.72f, 1f);
    private static readonly Color ElectricWhite = new Color(0.75f, 0.95f, 1f);

    private readonly List<SpriteRenderer> renderers = new();
    private readonly List<Color> originalColors = new();
    private float remainingDuration;
    private float elapsed;

    public static void Apply(GameObject target, float duration = 0.35f)
    {
        if (target == null) return;

        ElectrocutedEffect effect = target.GetComponent<ElectrocutedEffect>();
        if (effect == null) effect = target.AddComponent<ElectrocutedEffect>();
        effect.Play(duration);
    }

    private void Awake()
    {
        GetComponentsInChildren(true, renderers);
        foreach (SpriteRenderer renderer in renderers)
        {
            originalColors.Add(renderer.color);
        }
    }

    private void Play(float duration)
    {
        remainingDuration = Mathf.Max(remainingDuration, duration);
        elapsed = 0f;
    }

    private void Update()
    {
        if (remainingDuration <= 0f) return;

        elapsed += Time.deltaTime;
        remainingDuration -= Time.deltaTime;
        if (remainingDuration <= 0f)
        {
            RestoreColors();
            Destroy(this);
            return;
        }

        float pulse = Mathf.Abs(Mathf.Sin(elapsed * 55f));
        Color tint = Color.Lerp(ElectricBlue, ElectricWhite, pulse);
        float strength = Mathf.Lerp(0.45f, 0.85f, pulse);
        for (int i = 0; i < renderers.Count; i++)
        {
            Color original = originalColors[i];
            tint.a = original.a;
            renderers[i].color = Color.Lerp(original, tint, strength);
        }
    }

    private void OnDisable()
    {
        RestoreColors();
    }

    private void RestoreColors()
    {
        for (int i = 0; i < renderers.Count; i++)
        {
            if (renderers[i] != null) renderers[i].color = originalColors[i];
        }
    }
}

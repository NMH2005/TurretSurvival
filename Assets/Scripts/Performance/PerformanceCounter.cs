using TMPro;
using UnityEngine;

public class PerformanceCounter : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float updateInterval = 0.5f;

    private float timer;
    private int frames;

    private void Update()
    {
        frames++;
        timer += Time.unscaledDeltaTime;

        if (timer >= updateInterval)
        {
            float fps = frames / timer;
            float msPerFrame = (timer / frames) * 1000f;
            text.text = $"FPS: {fps:F0}\n{msPerFrame:F2} ms/frame";
            timer = 0f;
            frames = 0;
        }
    }
}
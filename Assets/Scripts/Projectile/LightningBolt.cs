using UnityEngine;

public class LightningBolt : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private int segments = 8;
    [SerializeField] private float jitter = 0.15f;
    [SerializeField] private float lifeTime = 0.15f;

    public void Draw(Vector2 from, Vector2 to)
    {
        lineRenderer.positionCount = segments + 1;
        Vector2 direction = to - from;
        Vector2 perpendicular = Vector2.Perpendicular(direction).normalized;

        for(int i = 0;i<=segments;i++)
        {
            float t = (float) i/segments;
            Vector2 point = Vector2.Lerp(from, to, t);

            if(i != 0 && i != segments)
            {
                point += perpendicular * Random.Range(-jitter, jitter);
            }

            lineRenderer.SetPosition(i, point);
        }

        Destroy(gameObject, lifeTime);
    }
}

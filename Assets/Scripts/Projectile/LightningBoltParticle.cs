using UnityEngine;

public class LightningBoltParticle : MonoBehaviour {
    [SerializeField] private ParticleSystem parti;
    [SerializeField] private float lifeTime = 0.3f;

    public void Draw(Vector2 start, Vector2 end)
    {
        var emitParams = new ParticleSystem.EmitParams();

        emitParams.position = start;
        parti.Emit(emitParams, 1);

        emitParams.position = end;
        parti.Emit(emitParams, 1);

        emitParams.position = (start + end) / 2;
        parti.Emit(emitParams, 1);
        Destroy(gameObject, lifeTime);
    }
}
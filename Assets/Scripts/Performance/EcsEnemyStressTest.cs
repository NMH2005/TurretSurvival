using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class EcsEnemyStressTest : MonoBehaviour {
    [SerializeField] private int spawnCount = 5000;
    [SerializeField] private float spawnRadius = 15f;
    [SerializeField] private float despawnDistance = 20f;

    private void Start()
    {
        var world = World.DefaultGameObjectInjectionWorld;
        var entityManager = world.EntityManager;

        var archetype = entityManager.CreateArchetype(
            typeof(LocalTransform),
            typeof(DespawnComponent)
        );

        for (int i = 0; i < spawnCount; i++)
        {
            var entity = entityManager.CreateEntity(archetype);

            float2 randomPos = UnityEngine.Random.insideUnitCircle * spawnRadius;

            entityManager.SetComponentData(entity, LocalTransform.FromPosition(randomPos.x, randomPos.y, 0));
            entityManager.SetComponentData(entity, new DespawnComponent
            {
                despawnDistance = despawnDistance,
                checkTimer = 0f
            });
        }

        Debug.Log($"Đã spawn {spawnCount} entity ECS để test.");
    }
}

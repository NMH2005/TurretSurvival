using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct DespawnSystem : ISystem {
    private float logTimer;

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.HasSingleton<PlayerPositionSingleton>()) return;

        float2 playerPos = SystemAPI.GetSingleton<PlayerPositionSingleton>().Position;
        float dt = SystemAPI.Time.DeltaTime;
        var ecb = new EntityCommandBuffer(
        state.WorldUpdateAllocator
    );
        foreach (var (transform, despawn, entity) in
                 SystemAPI.Query<RefRO<LocalTransform>, RefRW<DespawnComponent>>().WithEntityAccess())
        {
            despawn.ValueRW.checkTimer += dt;
            if (despawn.ValueRW.checkTimer < 0.5f) continue;
            despawn.ValueRW.checkTimer = 0f;

            float2 pos = transform.ValueRO.Position.xy;
            float dist = math.distance(pos, playerPos);

            if (dist > despawn.ValueRO.despawnDistance)
            {
                ecb.DestroyEntity(entity);
            }
        }
        ecb.Playback(state.EntityManager);

        logTimer += dt;

        if (logTimer >= 1f)
        {
            logTimer = 0f;

            int count = SystemAPI.QueryBuilder()
                .WithAll<DespawnComponent>()
                .Build()
                .CalculateEntityCount();

            Debug.Log($"ECS Entity còn lại: {count}");
        }
    }
}
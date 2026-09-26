using UnityEngine;
using Unity.Entities;
public class PlayerPositionBridge : MonoBehaviour {
    private EntityManager entityManager;
    private Entity singletonEntity;

    private void Start()
    {
        var world = World.DefaultGameObjectInjectionWorld;
        entityManager = world.EntityManager;

        singletonEntity = entityManager.CreateEntity();
        entityManager.AddComponentData(singletonEntity, new PlayerPositionSingleton());
    }

    private void Update()
    {
        entityManager.SetComponentData(singletonEntity, new PlayerPositionSingleton
        {
            Position = new Unity.Mathematics.float2(transform.position.x, transform.position.y)
        });
    }

    private void OnDestroy()
    {
        if (entityManager.Exists(singletonEntity))
        {
            entityManager.DestroyEntity(singletonEntity);
        }
    }
}
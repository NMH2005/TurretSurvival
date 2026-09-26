using Unity.Entities;

public struct DespawnComponent : IComponentData {
    public float despawnDistance;
    public float checkTimer;
}

using UnityEngine;

public class CoreComponent : MonoBehaviour {
    protected Core core;

    protected virtual void Awake()
    {
        core = transform.parent.GetComponent<Core>();
    }

    public virtual void LogicUpdate() { }
    public virtual void PhysicsUpdate() { }
}
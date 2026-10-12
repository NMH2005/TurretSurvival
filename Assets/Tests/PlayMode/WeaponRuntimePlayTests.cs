#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

public class WeaponRuntimePlayTests
{
    private readonly List<Object> cleanup = new List<Object>();

    private static Type Type(string name)
    {
        var type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType(name)).FirstOrDefault(t => t != null);
        Assert.That(type, Is.Not.Null, $"Missing runtime type: {name}");
        return type;
    }

    private static FieldInfo Field(object obj, string name)
    {
        for (var type = obj.GetType(); type != null; type = type.BaseType)
        {
            var field = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (field != null) return field;
        }
        throw new MissingFieldException(name);
    }

    private static object Get(object obj, string name) => obj.GetType().GetProperty(name).GetValue(obj);
    private static void Set(object obj, string name, object value) => Field(obj, name).SetValue(obj, value);
    private static object Call(object obj, string name, params object[] args) => obj.GetType()
        .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
        .Single(m => m.Name == name && m.GetParameters().Length == args.Length).Invoke(obj, args);
    private T Track<T>(T obj) where T : Object { cleanup.Add(obj); return obj; }

    private Component Weapon(string name)
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/Prefabs/Weapons/{name}.prefab");
        var go = Track(Object.Instantiate(prefab));
        var weapon = go.GetComponent(Type("WeaponBase"));
        ((Behaviour)weapon).enabled = false;
        var runtime = Type("WeaponRuntimeData").GetMethod("Create").Invoke(null, new object[] { weapon });
        Call(weapon, "Initialize", runtime, true);
        return weapon;
    }

    private Component Target(Vector2 position)
    {
        var root = Track(new GameObject("Damage target"));
        root.transform.position = position;
        root.layer = 7;
        root.AddComponent<BoxCollider2D>().isTrigger = true;
        var core = new GameObject("Core");
        core.transform.SetParent(root.transform, false);
        core.AddComponent(Type("Core"));
        var child = new GameObject("Combat");
        child.transform.SetParent(core.transform, false);
        var combat = child.AddComponent(Type("Combat"));
        var data = Track(ScriptableObject.CreateInstance(Type("EnemyData")));
        Set(data, "maxHealth", 1000);
        Set(combat, "enemyData", data);
        Call(combat, "Initialize");
        return combat;
    }

    private static Collider2D Collider(Component target) => target.GetComponentInParent<Collider2D>();

    [UnityTearDown]
    public IEnumerator Cleanup()
    {
        foreach (var bullet in Object.FindObjectsByType(Type("Projectile"), FindObjectsSortMode.None)) Object.Destroy(bullet);
        foreach (var obj in cleanup) if (obj != null) Object.Destroy(obj);
        cleanup.Clear();
        yield return null;
    }

    [UnityTest] public IEnumerator RifleDamageUsesSnapshot() => BulletSnapshot("Rifle", false);
    [UnityTest] public IEnumerator ShotgunPelletsUseSnapshot() => BulletSnapshot("ShotGun", false);
    [UnityTest] public IEnumerator LightningChainSurvivesProjectileAndWeapon() => BulletSnapshot("LightningBolt", true);

    private IEnumerator BulletSnapshot(string name, bool chain)
    {
        var weapon = Weapon(name);
        var runtime = Get(weapon, "RuntimeData");
        var projectileData = Get(runtime, "Projectile");
        Set(projectileData, "damage", 17);
        Set(projectileData, "speed", 2f);
        Set(projectileData, "lifeTime", 2f);
        var first = Target(new Vector2(100, 0));
        Component second = null, third = null;
        if (chain)
        {
            var chainData = Get(runtime, "Chain");
            Set(chainData, "damagePerJump", 7);
            Set(chainData, "chainCount", 2);
            Set(chainData, "chainRange", 1.5f);
            Set(chainData, "jumpDelay", 0.05f);
            var manager = Track(new GameObject("Chain manager")).AddComponent(Type("ChainLightningEffect"));
            var bolt = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Effects/ChainLightning.prefab")
                .GetComponent(Type("LightningBoltParticle"));
            Assert.That(bolt, Is.Not.Null);
            Set(manager, "lightning", bolt);
            second = Target(new Vector2(101, 0));
            third = Target(new Vector2(102, 0));
        }
        Physics2D.SyncTransforms();
        Call(weapon, "Fire");
        var bullets = Object.FindObjectsByType(Type("Projectile"), FindObjectsSortMode.None).Cast<Component>().ToArray();
        Assert.That(bullets.Length, Is.EqualTo(name == "ShotGun" ? (int)Field(Get(runtime, "Shotgun"), "pelletCount").GetValue(Get(runtime, "Shotgun")) : 1));
        Set(projectileData, "damage", 999);
        Set(projectileData, "speed", 999f);
        if (chain) Set(Get(runtime, "Chain"), "damagePerJump", 999);
        Object.Destroy(weapon.gameObject);
        yield return null;
        Assert.That(bullets[0].GetComponent<Rigidbody2D>().linearVelocity.magnitude, Is.EqualTo(2f).Within(0.01f));
        var hit = Type("Projectile").GetMethod("OnTriggerEnter2D", BindingFlags.NonPublic | BindingFlags.Instance);
        hit.Invoke(bullets[0], new object[] { Collider(first) });
        Assert.That(Get(first, "CurrentHealth"), Is.EqualTo(983));
        if (chain)
        {
            yield return new WaitForSeconds(0.15f);
            Assert.That(Get(second, "CurrentHealth"), Is.EqualTo(993));
            Assert.That(Get(third, "CurrentHealth"), Is.EqualTo(993));
        }
    }

    [UnityTest]
    public IEnumerator LaserUsesOwnedRuntimeTicks()
    {
        var weapon = Weapon("Laser");
        var data = Get(Get(weapon, "RuntimeData"), "Laser");
        Set(data, "damagePerTick", 9);
        Set(data, "tickRate", 1f);
        var effect = weapon.GetComponentInChildren(Type("LaserWeapon"));
        Assert.That(Field(effect, "laserData").GetValue(effect), Is.SameAs(data));
        var firePoint = (Transform)Field(effect, "firePoint").GetValue(effect);
        var target = Target((Vector2)firePoint.position + (Vector2)firePoint.up);
        Physics2D.SyncTransforms();
        Call(weapon, "StartFire");
        yield return null;
        yield return null;
        Call(weapon, "StopFire");
        Assert.That(Get(target, "CurrentHealth"), Is.EqualTo(991));
    }

    [UnityTest]
    public IEnumerator FlameUsesOwnedRuntimeDamageAndBurn()
    {
        var weapon = Weapon("FlameThrower");
        var data = Get(Get(weapon, "RuntimeData"), "Flame");
        Set(data, "damagePerTick", 7);
        Set(data, "tickRate", 1f);
        Set(data, "burnDamagePerTick", 3);
        Set(data, "burnTickRate", 0.05f);
        Set(data, "burnDuration", 1f);
        var effect = weapon.GetComponentInChildren(Type("FlamethrowerWeapon"));
        Assert.That(Field(effect, "flameData").GetValue(effect), Is.SameAs(data));
        var target = Target(new Vector2(100, 0));
        Call(weapon, "StartFire");
        Call(effect, "OnTriggerEnter2D", Collider(target));
        yield return null;
        yield return null;
        Call(weapon, "StopFire");
        int beforeBurn = (int)Get(target, "CurrentHealth");
        Assert.That(beforeBurn, Is.EqualTo(986));
        Set(data, "burnDamagePerTick", 999);
        Object.Destroy(weapon.gameObject);
        yield return new WaitForSeconds(0.13f);
        int burnDamage = beforeBurn - (int)Get(target, "CurrentHealth");
        Assert.That(burnDamage, Is.GreaterThanOrEqualTo(3));
        Assert.That(burnDamage % 3, Is.Zero);
    }
}
#endif

using NUnit.Framework;
using UnityEditor;
using UnityEngine;

public class WeaponRuntimeDataTests
{
    private static Component Source(string name) => AssetDatabase
        .LoadAssetAtPath<GameObject>($"Assets/Prefabs/Weapons/{name}.prefab")
        .GetComponent(TestAccess.Type("WeaponBase"));

    [Test]
    public void CloneDoesNotChangeTemplateOrSibling()
    {
        var source = Source("Rifle");
        var template = TestAccess.Field(source, "weaponData").GetValue(source);
        var projectile = TestAccess.Field(template, "projectilePrefab").GetValue(template);
        var templateData = TestAccess.Field(projectile, "projectData").GetValue(projectile);
        int original = (int)TestAccess.Field(templateData, "damage").GetValue(templateData);
        var a = TestAccess.Create(source);
        var b = TestAccess.Create(source);
        try
        {
            var cloneA = TestAccess.Get(a, "Projectile");
            var cloneB = TestAccess.Get(b, "Projectile");
            TestAccess.Field(cloneA, "damage").SetValue(cloneA, original + 5);
            Assert.That(TestAccess.Field(templateData, "damage").GetValue(templateData), Is.EqualTo(original));
            Assert.That(TestAccess.Field(cloneB, "damage").GetValue(cloneB), Is.EqualTo(original));
            Assert.That(TestAccess.Get(a, "Weapon"), Is.Not.SameAs(template));
        }
        finally { TestAccess.Dispose(a); TestAccess.Dispose(b); }
    }

    [Test]
    public void DifferentWeaponDefinitionsCanShareProjectileTemplate()
    {
        var source = Source("Rifle");
        var go = new GameObject("Alternate definition");
        go.SetActive(false);
        var alternate = go.AddComponent(TestAccess.Type("WeaponBase"));
        var definition = Object.Instantiate((ScriptableObject)TestAccess.Field(source, "weaponData").GetValue(source));
        TestAccess.Field(alternate, "weaponData").SetValue(alternate, definition);
        var a = TestAccess.Create(source);
        var b = TestAccess.Create(alternate);
        try
        {
            var aData = TestAccess.Get(a, "Projectile");
            var bData = TestAccess.Get(b, "Projectile");
            int original = (int)TestAccess.Field(bData, "damage").GetValue(bData);
            TestAccess.Field(aData, "damage").SetValue(aData, original + 5);
            Assert.That(TestAccess.Field(bData, "damage").GetValue(bData), Is.EqualTo(original));
        }
        finally
        {
            TestAccess.Dispose(a); TestAccess.Dispose(b);
            Object.DestroyImmediate(go); Object.DestroyImmediate(definition);
        }
    }

    [TestCase("Rifle", "Projectile")]
    [TestCase("Sniper", "Projectile")]
    [TestCase("ShotGun", "Shotgun")]
    [TestCase("Laser", "Laser")]
    [TestCase("FlameThrower", "Flame")]
    [TestCase("LightningBolt", "Chain")]
    public void ExistingWeaponHasIsolatedSpecializedData(string prefab, string property)
    {
        var a = TestAccess.Create(Source(prefab));
        var b = TestAccess.Create(Source(prefab));
        try
        {
            Assert.That(TestAccess.Get(a, property), Is.Not.Null);
            Assert.That(TestAccess.Get(a, property), Is.Not.SameAs(TestAccess.Get(b, property)));
        }
        finally { TestAccess.Dispose(a); TestAccess.Dispose(b); }
    }

    [Test]
    public void DisposeReleasesClonesAndNewRunStartsFromTemplate()
    {
        var source = Source("Rifle");
        var a = TestAccess.Create(source);
        var projectile = (Object)TestAccess.Get(a, "Projectile");
        int original = (int)TestAccess.Field(projectile, "damage").GetValue(projectile);
        TestAccess.Field(projectile, "damage").SetValue(projectile, original + 5);
        TestAccess.Dispose(a);
        TestAccess.Dispose(a);
        Assert.That(projectile == null, Is.True);
        var nextRun = TestAccess.Create(source);
        try
        {
            var next = TestAccess.Get(nextRun, "Projectile");
            Assert.That(TestAccess.Field(next, "damage").GetValue(next), Is.EqualTo(original));
        }
        finally { TestAccess.Dispose(nextRun); }
    }
}

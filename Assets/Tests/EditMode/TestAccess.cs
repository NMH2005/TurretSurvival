using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

internal static class TestAccess
{
    internal static Type Type(string name)
    {
        var type = AppDomain.CurrentDomain.GetAssemblies()
            .Select(assembly => assembly.GetType(name)).FirstOrDefault(value => value != null);
        Assert.That(type, Is.Not.Null, $"Missing runtime type: {name}");
        return type;
    }

    internal static object Get(object target, string property) =>
        target.GetType().GetProperty(property).GetValue(target);

    internal static FieldInfo Field(object target, string name)
    {
        for (var type = target.GetType(); type != null; type = type.BaseType)
        {
            var field = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (field != null) return field;
        }
        throw new MissingFieldException(target.GetType().Name, name);
    }

    internal static object Create(Component source) =>
        Type("WeaponRuntimeData").GetMethod("Create").Invoke(null, new object[] { source });

    internal static void Dispose(object runtime) => ((IDisposable)runtime).Dispose();
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MapperValidator.Commons;

static class ObjectComparer
{
    public static bool IsEqual(object? objA, object? objB)
    {
        if (IsOneElementNull(objA, objB))
            return false;

        if (IsEqualNull(objA, objB))
            return true;

        if (typeof(IComparable).IsAssignableFrom(objA?.GetType()))
            return ((IComparable)objA).CompareTo(objB) == 0;

        if (typeof(IComparable).IsAssignableFrom(objB?.GetType()))
            return ((IComparable)objB).CompareTo(objA) == 0;

        if (IsPrimitive(objA) && IsPrimitive(objB))
            return IsEqualPrimitive(objA, objB);

        if (IsArray(objA) && IsArray(objB))
            return IsEqualArray(objA, objB);

        if (IsList(objA) && IsList(objB))
            return IsEqualList(objA, objB);

        return IsEqualObject(objA, objB);
    }

    public static bool IsEqualNull(object? objA, object? objB)
    {
        return objA == null && objB == null;
    }

    public static bool IsOneElementNull(object? objA, object? objB)
    {
        return objA == null && objB != null || objA != null && objB == null;
    }

    public static bool IsClass(object obj)
    {
        return obj.GetType().IsClass;
    }

    public static bool IsPrimitive(object? obj)
    {
        return obj?.GetType().IsPrimitive ?? false;
    }

    public static bool IsArray(object? obj)
    {
        return obj?.GetType().IsArray ?? false;
    }

    public static bool IsList(object? obj)
    {
        return typeof(IList).IsAssignableFrom(obj?.GetType());
    }

    public static bool IsDictionnary(object? obj)
    {
        return typeof(IDictionary).IsAssignableFrom(obj?.GetType());
    }

    public static bool IsEqualPrimitive(object? a, object? b)
    {
        return a?.Equals(b) ?? false;
    }

    public static bool IsEqualArray(object? a, object? b)
    {
        var arrayA = a as Array;
        var arrayB = b as Array;

        if (arrayA == null || arrayB == null)
            throw new InvalidCastException();

        if (arrayA.Length != arrayB.Length)
            return false;

        if (arrayA.GetType().GetElementType() != arrayB.GetType().GetElementType())
            return false;

        for (int i = 0; i < arrayA.Length; i++)
        {
            if (!IsEqual(arrayA.GetValue(i), arrayB.GetValue(i)))
                return false;
        }

        return true;
    }

    public static bool IsEqualList(object? a, object? b)
    {
        var listA = a as IList;
        var listB = b as IList;

        if (listA == null || listB == null)
            throw new InvalidCastException();

        if (listA.Count != listB.Count)
            return false;

        if (listA.GetType().GetGenericArguments()[0] != listB.GetType().GetGenericArguments()[0])
            return false;

        for (int i = 0; i < listA.Count; i++)
        {
            if (!IsEqual(listA[i], listB[i]))
                return false;
        }

        return true;
    }

    public static bool IsEqualObject(object? a, object? b)
    {
        if (IsOneElementNull(a, b))
            return false;

        if (IsEqualNull(a, b))
            return true;

        if (a.GetType() != b.GetType())
            return false;

        Type m_type = a.GetType();
        foreach (var field in m_type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            var objA = field.GetValue(a);
            var objB = field.GetValue(b);

            if (!IsEqual(objA, objB))
            {
                return false;
            }
        }

        foreach (var property in m_type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            var objA = property.GetValue(a);
            var objB = property.GetValue(b);

            if (!IsEqual(objA, objB))
            {
                return false;
            }
        }

        return true;
    }
}



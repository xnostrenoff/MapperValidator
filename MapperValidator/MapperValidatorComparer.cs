using MapperValidator.Commons;
using MapperValidator.Exceptions;
using MapperValidator.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MapperValidator;

public class MapperValidatorComparer<TSource, TCompare> : IEqualityComparer
{
    #region Variables

    private MapperTester _parent;
    private MapperValidatorComparerConfiguration<TSource, TCompare> _configuration;

    #endregion Variables

    public MapperValidatorComparer(MapperValidatorComparerConfiguration<TSource, TCompare> configuration, MapperTester parent)
    {
        _parent = parent;
        _configuration = configuration;
    }

    bool IEqualityComparer.Compare(object objSource, object objCompare)
    {
        var destProperties = typeof(TCompare).GetProperties(BindingFlags.Instance | BindingFlags.Public);

        foreach (var destProperty in destProperties)
        {
            if (_configuration.IsPropertyIgnored (destProperty.Name))
                continue;

            var associatedPropertyName = _configuration.GetAssociateSourcedProperty(destProperty.Name);
            if (associatedPropertyName != null)
            {
                var associatedProperty = typeof(TSource).GetProperty(associatedPropertyName, BindingFlags.Instance | BindingFlags.Public);
                if (associatedProperty == null)
                    throw new AnalyzeException($"Missing {typeof(TSource).Name}.{associatedPropertyName} source property for {typeof(TCompare).Name}.{destProperty.Name}.");
                CompareProperties(objSource, associatedProperty, objCompare, destProperty);
                continue;
            }

            if (!_configuration.IgnoreNotConfiguredProperties)
            {
                var sourceProperty = typeof(TSource).GetProperty(destProperty.Name, BindingFlags.Instance | BindingFlags.Public);
                if (sourceProperty == null)
                    continue;
                CompareProperties(objSource, sourceProperty, objCompare, destProperty);
            }

        }

        return true;
    }

    private void CompareProperties(object sourceObject, PropertyInfo sourceProperty, object destinationObject, PropertyInfo destinationProperty)
    {
        var sourceValue = sourceProperty.GetValue(sourceObject);
        var destinationValue = destinationProperty.GetValue(destinationObject);

        var objComparer = new ObjectComparer(_parent);
        if (!objComparer.IsEqual(sourceValue, destinationValue))
        {
            string message = $"{typeof(TCompare).Name}.{destinationProperty.Name} '{sourceValue?.ToString() ?? "<null>"}' attended but was '{destinationValue?.ToString() ?? "<null>"}'";
            throw new AnalyzeException(message);
        }
    }
}
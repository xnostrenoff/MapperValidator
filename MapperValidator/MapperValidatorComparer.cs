using MapperValidator.Commons;
using MapperValidator.Exceptions;
using MapperValidator.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MapperValidator;

public class MapperValidatorComparer<TSource, TDest> : IEqualityComparer
{
    #region Variables

    private MapperTester _parent;
    private MapperValidatorComparerConfiguration<TSource, TDest> _configuration;

    #endregion Variables

    public MapperValidatorComparer(MapperValidatorComparerConfiguration<TSource, TDest> configuration, MapperTester parent)
    {
        _parent = parent;
        _configuration = configuration;
    }

    bool IEqualityComparer.Compare(object objSource, object objDest)
    {
        var destProperties = typeof(TDest).GetProperties(BindingFlags.Instance | BindingFlags.Public);

        foreach (var destProperty in destProperties)
        {
            if (_configuration.IsPropertyIgnored (destProperty.Name))
                continue;

            var associatedSourcePropertyName = _configuration.GetAssociateSourceProperty(destProperty.Name);
            if (associatedSourcePropertyName != null)
            {
                var associatedSourceProperty = typeof(TSource).GetProperty(associatedSourcePropertyName, BindingFlags.Instance | BindingFlags.Public);
                if (associatedSourceProperty == null)
                    throw new AnalyzeException($"Missing {typeof(TSource).Name}.{associatedSourcePropertyName} source property for {typeof(TDest).Name}.{destProperty.Name}.");
                CompareProperties(objSource, associatedSourceProperty, objDest, destProperty);
                continue;
            }

            if (!_configuration.IgnoreNotConfiguredProperties)
            {
                var sourceProperty = typeof(TSource).GetProperty(destProperty.Name, BindingFlags.Instance | BindingFlags.Public);
                if (sourceProperty == null)
                    continue;
                CompareProperties(objSource, sourceProperty, objDest, destProperty);
            }

        }

        return true;
    }

    private void CompareProperties(object sourceObject, PropertyInfo sourceProperty, object destObject, PropertyInfo destProperty)
    {
        var sourceValue = sourceProperty.GetValue(sourceObject);
        var destinationValue = destProperty.GetValue(destObject);

        var objComparer = new ObjectComparer(_parent);
        if (!objComparer.IsEqual(sourceValue, destinationValue))
        {
            string message = $"{typeof(TDest).Name}.{destProperty.Name} '{sourceValue?.ToString() ?? "<null>"}' attended but was '{destinationValue?.ToString() ?? "<null>"}'";
            throw new AnalyzeException(message);
        }
    }
}
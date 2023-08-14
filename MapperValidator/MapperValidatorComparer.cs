using MapperValidator.Commons;
using MapperValidator.Exceptions;
using MapperValidator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MapperValidator;

public class MapperValidatorComparer<TSource, TCompare> : IEqualityComparer
{
    #region Variables

    private bool _ignoreNotConfiguredProperties = false;
    private Dictionary<string, string> _associatedProperties = new Dictionary<string, string>();
    private List<string> _ignoredProperties = new();

    #endregion Variables

    #region Methods

    public virtual MapperValidatorComparer<TSource, TCompare> Associate<TSourceProperty, TCompareProperty>(
        Expression<Func<TSource, TSourceProperty>> sourceProperty,
        Expression<Func<TCompare, TCompareProperty>> compareProperty)
    {
        var sourcePropertyName = PropertyName.For(sourceProperty);
        var destinationPropertyName = PropertyName.For(compareProperty);

        _associatedProperties.Add(destinationPropertyName, sourcePropertyName);
        return this;
    }

    public MapperValidatorComparer<TSource, TCompare> Ignore<TCompareProperty>(Expression<Func<TCompare, TCompareProperty>> compareProperty)
    {
        var ignoredPropertyName = PropertyName.For(compareProperty);
        _ignoredProperties.Add(ignoredPropertyName);

        return this;
    }

    public MapperValidatorComparer<TSource, TCompare> IgnoreNotAssociatedProperties()
    {
        _ignoreNotConfiguredProperties = true;
        return this;
    }

    public MapperValidatorComparer<TSource, TCompare> IncludeNotAssociatedProperties()
    {
        _ignoreNotConfiguredProperties = false;
        return this;
    }

    #endregion Methods

    #region IEqualityAssertComparer members

    bool IEqualityComparer.Compare(object objSource, object objCompare)
    {
        var destProperties = typeof(TCompare).GetProperties(BindingFlags.Instance | BindingFlags.Public);

        foreach (var destProperty in destProperties)
        {
            if (_ignoredProperties.Contains(destProperty.Name))
                continue;

            _associatedProperties.TryGetValue(destProperty.Name, out var associatedPropertyName);
            if (associatedPropertyName != null)
            {
                var associatedProperty = typeof(TSource).GetProperty(associatedPropertyName, BindingFlags.Instance | BindingFlags.Public);
                if (associatedProperty == null)
                    throw new AnalyzeException($"Missing {typeof(TSource).Name}.{associatedPropertyName} source property for {typeof(TCompare).Name}.{destProperty.Name}.");
                CompareProperties(objSource, associatedProperty, objCompare, destProperty);
                continue;
            }

            if (!_ignoreNotConfiguredProperties)
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

        if (!ObjectComparer.IsEqual(sourceValue, destinationValue))
        {
            string message = $"{nameof(TCompare)}.{destinationProperty.Name} '{sourceValue?.ToString() ?? "<null>"}' attended but was '{destinationValue?.ToString() ?? "<null>"}'";
            throw new AnalyzeException(message);
        }
    }

    #endregion IEqualityAssertComparer members
}

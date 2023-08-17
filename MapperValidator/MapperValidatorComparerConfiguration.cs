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

public class MapperValidatorComparerConfiguration<TSource, TDest> : IEqualityComparerConfiguration
{
    #region Variables

    internal Dictionary<string, string> _associatedProperties = new Dictionary<string, string>();
    internal List<string> _ignoredProperties = new();

    #endregion Variables

    #region Properties

    internal bool IgnoreNotConfiguredProperties { get; private set; } = false;

    #endregion Properties

    #region Methods

    public virtual MapperValidatorComparerConfiguration<TSource, TDest> Associate<TSourceProperty, TDestProperty>(
        Expression<Func<TSource, TSourceProperty>> sourceProperty,
        Expression<Func<TDest, TDestProperty>> compareProperty)
    {
        var sourcePropertyName = PropertyName.For(sourceProperty);
        var destinationPropertyName = PropertyName.For(compareProperty);

        _associatedProperties.Add(destinationPropertyName, sourcePropertyName);
        return this;
    }


    internal string? GetAssociateSourceProperty(string destPropertyName)
    {
        if (_associatedProperties.TryGetValue(destPropertyName, out var associatedPropertyName))
            return associatedPropertyName;
        return null;
    }

    public MapperValidatorComparerConfiguration<TSource, TDest> Ignore<TDestProperty>(Expression<Func<TDest, TDestProperty>> compareProperty)
    {
        var ignoredPropertyName = PropertyName.For(compareProperty);
        _ignoredProperties.Add(ignoredPropertyName);

        return this;
    }

    public MapperValidatorComparerConfiguration<TSource, TDest> IgnoreNotAssociatedProperties()
    {
        IgnoreNotConfiguredProperties = true;
        return this;
    }

    public MapperValidatorComparerConfiguration<TSource, TDest> IncludeNotAssociatedProperties()
    {
        IgnoreNotConfiguredProperties = false;
        return this;
    }

    internal bool IsPropertyIgnored(string destinationPropertyName)
    {
        return _ignoredProperties.Contains(destinationPropertyName);
    }

    #endregion Methods

    #region IEqualityAssertComparer members

    IEqualityComparer IEqualityComparerConfiguration.Build(MapperValidator.MapperTester tester)
    {
        return new MapperValidatorComparer<TSource, TDest>(this, tester);
    }

    #endregion IEqualityAssertComparer members
}

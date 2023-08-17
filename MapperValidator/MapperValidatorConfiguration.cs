using MapperValidator.Exceptions;
using MapperValidator.Interfaces;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapperValidator;

public class MapperValidatorConfiguration
{
    private Dictionary<Tuple<Type, Type>, IEqualityComparerConfiguration> _comparers = new();
    private bool _authorizeUnmanagedTypes = false;

    public MapperValidatorConfiguration EnableUnmanagerTypeComparaison()
    {
        _authorizeUnmanagedTypes = true;
        return this;
    }

    public MapperValidatorConfiguration DisableUnmanagerTypeComparaison()
    {
        _authorizeUnmanagedTypes = false;
        return this;
    }

    public MapperValidatorComparerConfiguration<TSource, TMapped> AddComparer<TSource, TMapped>()
    {
        var comparer = new MapperValidatorComparerConfiguration<TSource, TMapped>();

        var key = new Tuple<Type, Type>(typeof(TSource), typeof(TMapped));
        _comparers.Add(key, comparer);

        return comparer;
    }

    internal IEqualityComparer GetComparer(Type typeSource, Type typeCompare, MapperTester tester)
    {
        var key = new Tuple<Type, Type>(typeSource, typeCompare);
        if (_comparers.TryGetValue(key, out var comparer))
            return comparer.Build (tester);

        if (_authorizeUnmanagedTypes)
            return CreateDefaultComparer(typeSource, typeCompare)
                .Build (tester);
        
        Assert.Fail($"<{typeSource.Name},{typeCompare}> not configured.{Environment.NewLine}Add {nameof(EnableUnmanagerTypeComparaison)} on your configaration or configure it.");
        throw new AnalyzeException();
    }

    private static IEqualityComparerConfiguration CreateDefaultComparer(Type sourceType, Type destinationType)
    {
        var comparerType = typeof(MapperValidatorComparerConfiguration<,>);
        comparerType = comparerType.MakeGenericType(sourceType, destinationType);
        return Activator.CreateInstance(comparerType) as IEqualityComparerConfiguration
            ?? throw new InvalidOperationException("");
    }
}

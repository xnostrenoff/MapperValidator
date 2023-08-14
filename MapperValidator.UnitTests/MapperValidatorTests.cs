using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MapperValidator.UnitTests;

public class MapperValidatorTests
{
    private class ClassSource
    {
        public int SourceId { get; set; } = 1;

        public int[]? Array { get; set; } = null;

        public int SameName { get; set; } = 1;
    }

    private class ClassCompare
    {
        public int CompareId { get; set; } = 1;

        public int[]? Array { get; set; } = null;

        public int SameName { get; set; } = 1;
    }

    [Test]
    public void AssignsEquality()
    {
        var config = new MapperValidatorConfiguration();
        config.AddComparer<ClassSource, ClassCompare>()
            .Associate(src => src.SourceId, cmp => cmp.CompareId)
            .Ignore(dst => dst.Array);

        var source = new ClassSource { SourceId = 1, Array = new[] { 1, 2, 3 } };
        var dest = new ClassCompare { CompareId = 1 };

        var assetComparer = new MapperValidator(config);
        assetComparer.IsEqual(source, dest);
    }

    [Test]
    public void CompareWithDestNullValue()
    {
        var config = new MapperValidatorConfiguration();
        config.AddComparer<ClassSource, ClassCompare>()
            .Associate(src => src.Array, cmp => cmp.Array)
            .IgnoreNotAssociatedProperties();

        var source = new ClassSource { Array = new[] { 1, 2, 3 } };
        var dest = new ClassCompare { };

        var assetComparer = new MapperValidator(config);
        assetComparer.IsNotEqual(source, dest);
    }

    [Test]
    public void CompareUnconfiguredClasses()
    {
        var config = new MapperValidatorConfiguration()
            .EnableUnmanagerTypeComparaison();

        var source = new ClassSource();
        var dest = new ClassCompare();

        var assetComparer = new MapperValidator(config);
        assetComparer.IsEqual(source, dest);
    }
}

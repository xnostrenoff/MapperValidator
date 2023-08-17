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
    enum TestEnum
    {
        Value0 = 0,
        Value1 = 1,
        Value2 = 2
    }

    private class ClassSource
    {
        public int SourceId { get; set; } = 1;

        public int[]? Array { get; set; } = null;

        public int SameName { get; set; } = 1;

        public ClassSourceB Sub { get; set; } = new ClassSourceB { Id = 25 };

        public int IntEnumValue { get; set; } = 1;

        public TestEnum EnumValue { get; set; } = TestEnum.Value1;
    }

    private class ClassSourceB
    {
        public int Id { get; set; } = 1;
    }

    private class ClassDestB
    {
        public int Id { get; set; } = 1;
    }

    private class ClassCompare
    {
        public int CompareId { get; set; } = 1;

        public int[]? Array { get; set; } = null;

        public int SameName { get; set; } = 1;

        public ClassDestB Sub { get; set; } = new ClassDestB { Id = 25 };

        public TestEnum EnumValue { get; set; } = TestEnum.Value1;
    }

    [Test]
    public void IsEqualWithAssociations()
    {
        var config = new MapperValidatorConfiguration();
        config.AddComparer<ClassSource, ClassCompare>()
            .Associate(src => src.SourceId, cmp => cmp.CompareId)
            .Ignore(dst => dst.Array)
            .Ignore(dst => dst.Sub);

        var source = new ClassSource { SourceId = 1, Array = new[] { 1, 2, 3 } };
        var dest = new ClassCompare { CompareId = 1 };

        var assetComparer = new MapperTester(config);
        assetComparer.IsEqual(source, dest);
    }

    [Test]
    public void CompareWithDestinationValueIsNull()
    {
        var config = new MapperValidatorConfiguration();
        config.AddComparer<ClassSource, ClassCompare>()
            .Associate(src => src.Array, cmp => cmp.Array)
            .IgnoreNotAssociatedProperties();

        var source = new ClassSource { Array = new[] { 1, 2, 3 } };
        var dest = new ClassCompare { };

        var assetComparer = new MapperTester(config);
        assetComparer.IsNotEqual(source, dest);
    }

    [Test]
    public void CompareUnconfiguredClasses()
    {
        var config = new MapperValidatorConfiguration()
            .EnableUnmanagerTypeComparaison();

        var source = new ClassSource();
        var dest = new ClassCompare();

        var assetComparer = new MapperTester(config);
        assetComparer.IsEqual(source, dest);
    }

    [Test]
    public void UsingRecurciveMapperTesterDuringComparaisonProcess()
    {
        var config = new MapperValidatorConfiguration();
        config.AddComparer<ClassSource, ClassCompare>()
            .Associate(src => src.Sub, cmp => cmp.Sub)
            .IgnoreNotAssociatedProperties();

        config.AddComparer<ClassSourceB, ClassDestB>()
            .Associate(src => src.Id, cmp => cmp.Id)
            .IgnoreNotAssociatedProperties();

        var source = new ClassSource();
        var dest = new ClassCompare();

        var assetComparer = new MapperTester(config);
        assetComparer.IsEqual(source, dest);
    }

    [Test]
    public void UsingRecurciveMapperTesterUnmanagerTypesDuringComparaisonProcess()
    {
        var config = new MapperValidatorConfiguration()
            .EnableUnmanagerTypeComparaison();
            ;
        config.AddComparer<ClassSource, ClassCompare>()
            .Associate(src => src.Sub, cmp => cmp.Sub)
            .IgnoreNotAssociatedProperties();

        var source = new ClassSource();
        var dest = new ClassCompare();

        var assetComparer = new MapperTester(config);
        assetComparer.IsEqual(source, dest);
    }

    [Test]
    public void TestEnums()
    {
        var config = new MapperValidatorConfiguration();

        config.AddComparer<ClassSource, ClassCompare>()
            .Associate(src => src.EnumValue, cmp => cmp.EnumValue)
            .IgnoreNotAssociatedProperties();

        var source = new ClassSource();
        var dest = new ClassCompare();

        var assetComparer = new MapperTester(config);
        assetComparer.IsEqual(source, dest);
    }

    [Test]
    public void TestEnumWithInt ()
    {
        var config = new MapperValidatorConfiguration();
        
        config.AddComparer<ClassSource, ClassCompare>()
            .Associate(src => src.IntEnumValue, cmp => cmp.EnumValue)
            .IgnoreNotAssociatedProperties();

        var source = new ClassSource();
        var dest = new ClassCompare();

        var assetComparer = new MapperTester(config);
        assetComparer.IsEqual(source, dest);
    }
}

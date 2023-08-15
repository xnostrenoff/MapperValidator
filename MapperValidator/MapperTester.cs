using MapperValidator.Exceptions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapperValidator;

public class MapperTester
{
    private MapperValidatorConfiguration _configuration;

    public MapperTester(MapperValidatorConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void IsEqual(object objA, object objB)
    {
        var typeA = objA.GetType();
        var typeB = objB.GetType();

        try
        {
            var comparer = _configuration.GetComparer(typeA, typeB);
            if (comparer == null)
                throw new Exception();

            comparer.Compare(objA, objB);
        }
        catch (AnalyzeException ex)
        {
            Assert.Fail(ex.Message);
        }
    }

    public void IsNotEqual(object objA, object objB)
    {
        var typeA = objA.GetType();
        var typeB = objB.GetType();

        try
        {
            var comparer = _configuration.GetComparer(typeA, typeB);
            if (comparer == null)
                throw new Exception();

            comparer.Compare(objA, objB);
        }
        catch (AnalyzeException ex)
        {
            Assert.Pass(ex.Message);
        }
    }
}

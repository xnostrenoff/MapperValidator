using System;
using System.Collections.Generic;
using System.Text;

namespace MapperValidator.Interfaces
{
    internal interface IEqualityComparerConfiguration
    {
        IEqualityComparer Build(MapperTester tester);
    }
}

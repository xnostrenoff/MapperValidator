using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapperValidator.Interfaces;

internal interface IEqualityComparer
{
    bool Compare(object objA, object objB);

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapperValidator.Exceptions;

internal class AnalyzeException : Exception
{
    public AnalyzeException() : base() { }

    public AnalyzeException(string message)
        : base(message) { }
}
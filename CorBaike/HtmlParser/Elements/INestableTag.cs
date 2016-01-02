using System;
using System.Collections.Generic;

namespace HtmlSharp.Elements
{
    public interface IAllowsNestingSelf
    {
        IEnumerable<Type> NestingBreakers { get; }
    }
}

using System.Collections.Generic;

namespace HtmlSharp.Css
{
    public abstract class Expression
    {
        public abstract IEnumerable<int> GetValues();
    }
}

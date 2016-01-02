using HtmlSharp.Elements;
using System.Collections.Generic;

namespace HtmlSharp.Css
{
    public abstract class Combinator : IFilter
    {
        public override bool Equals(object obj)
        {
            return obj != null && GetType() == obj.GetType();
        }

        public override int GetHashCode()
        {
            return GetType().GetHashCode();
        }

        public abstract IEnumerable<Tag> Apply(IEnumerable<Tag> tags);
    }
}

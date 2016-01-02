using HtmlSharp.Elements;
using System.Collections.Generic;

namespace HtmlSharp.Css
{
    public class UniversalSelector : TypeSelector
    {
        public UniversalSelector()
            : base("*")
        {

        }

        public UniversalSelector(SelectorNamespacePrefix prefix)
            : base("*", prefix)
        {

        }

        public override string ToString()
        {
            return "*";
        }

        public override IEnumerable<Tag> Apply(IEnumerable<Tag> tags)
        {
            return tags;
        }
    }
}

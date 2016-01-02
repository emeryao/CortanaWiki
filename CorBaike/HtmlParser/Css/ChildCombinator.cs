using HtmlSharp.Elements;
using System.Collections.Generic;

namespace HtmlSharp.Css
{
    public class ChildCombinator : Combinator
    {
        public override string ToString()
        {
            return ">";
        }

        public override IEnumerable<Tag> Apply(IEnumerable<Tag> tags)
        {
            foreach (var tag in tags)
            {
                foreach (var child in tag.Children)
                {
                    Tag childTag = child as Tag;
                    if (childTag != null)
                    {
                        yield return childTag;
                    }
                }
            }
        }
    }
}

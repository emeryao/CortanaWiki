using HtmlSharp.Elements;
using System.Collections.Generic;

namespace HtmlSharp.Css
{
    public class GeneralSiblingCombinator : Combinator
    {
        public override string ToString()
        {
            return "~";
        }

        public override IEnumerable<Tag> Apply(IEnumerable<Tag> tags)
        {
            foreach (var tag in tags)
            {
                var sibling = tag.NextSibling;
                while (sibling != null)
                {
                    Tag siblingTag = sibling as Tag;
                    if (siblingTag != null)
                    {
                        yield return siblingTag;
                    }
                    sibling = sibling.NextSibling;
                }
            }
        }
    }
}

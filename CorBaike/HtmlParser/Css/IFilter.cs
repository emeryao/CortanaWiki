using HtmlSharp.Elements;
using System.Collections.Generic;

namespace HtmlSharp.Css
{
    public interface IFilter
    {
        IEnumerable<Tag> Apply(IEnumerable<Tag> tags);
    }
}

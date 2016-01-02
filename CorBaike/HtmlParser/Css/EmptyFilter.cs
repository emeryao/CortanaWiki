﻿using HtmlSharp.Elements;
using System.Collections.Generic;
using System.Linq;

namespace HtmlSharp.Css
{
    public class EmptyFilter : IFilter
    {
        public override bool Equals(object obj)
        {
            return obj != null && GetType() == obj.GetType();
        }

        public override int GetHashCode()
        {
            return GetType().GetHashCode();
        }

        public IEnumerable<Tag> Apply(IEnumerable<Tag> tags)
        {
            return tags.Where(tag => tag.Children.Count == 0);
        }
    }
}

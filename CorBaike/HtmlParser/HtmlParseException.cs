using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HtmlSharp
{
    public class HtmlParseException : Exception
    {
        public HtmlParseException()
        {
        }

        public HtmlParseException(string message)
            : base(message)
        {
        }

        public HtmlParseException(string message, Exception inner)
            : base(message, inner)
        {
        }

    }
}

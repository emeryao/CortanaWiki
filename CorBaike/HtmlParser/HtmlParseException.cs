using System;

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

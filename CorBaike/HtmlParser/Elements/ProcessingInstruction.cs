namespace HtmlSharp.Elements
{
    public class ProcessingInstruction : HtmlText
    {
        public override string ToString()
        {
            return "<?" + base.ToString() + "?>";
        }
    }
}

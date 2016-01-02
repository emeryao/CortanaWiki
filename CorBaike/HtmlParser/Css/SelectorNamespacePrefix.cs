namespace HtmlSharp.Css
{
    public class SelectorNamespacePrefix
    {
        public string Namespace { get; private set; }
        public SelectorNamespacePrefix(string ns)
        {
            this.Namespace = ns;
        }
    }
}

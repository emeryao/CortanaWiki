namespace HtmlSharp.Elements
{
    public class Comment : HtmlText
    {
        public override string ToString()
        {
            return "<!--" + base.ToString() + "-->";
        }
    }
}

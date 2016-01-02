namespace HtmlSharp.Css
{
    public class CheckedFilter : AttributeFilter
    {
        public CheckedFilter()
            : base("checked")
        {
        }

        public override bool Equals(object obj)
        {
            return obj != null && GetType() == obj.GetType();
        }

        public override int GetHashCode()
        {
            return GetType().GetHashCode();
        }
    }
}

namespace HtmlSharp.Css
{
    public class LastOfTypeFilter : NthLastOfTypeFilter
    {
        public LastOfTypeFilter()
            : base(new NumericExpression(0, 1))
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

﻿
// ReSharper disable once CheckNamespace
namespace ExCSS
{
    public class Term
    {
        protected string Text;
        private static Term _inherited;

        public Term()
        {
            RuleValueType = RuleValueType.Custom;
        }

        public static Term Inherit
        {
            get
            {
                return _inherited ?? (_inherited = new Term
                {
                    Text = "inherit",
                    RuleValueType = RuleValueType.Inherit
                });
            }
        }

        public RuleValueType RuleValueType { get; internal set; }

        public override string ToString()
        {
            return Text;
        }
    }
}

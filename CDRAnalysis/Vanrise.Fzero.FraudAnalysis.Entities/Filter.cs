using System;
using Vanrise.Fzero.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public enum CriteriaCompareOperator { GreaterThanorEqual, LessThanorEqual }

    public class Filter
    {
        public bool ExcludeHourly { get; set; }

        public int FilterId { get; set; }

        public CriteriaCompareOperator CompareOperator { get; set; }

        public string Description { get; set; }

        public string Abbreviation { get; set; }

        public string Label { get; set; }

        public Func<NumberProfile, decimal?> Expression;

        public decimal MinValue { get; set; }

        public decimal MaxValue { get; set; }

        public int DecimalPrecision { get; set; }

        public string ToolTip { get; set; }

        public OperatorType OperatorTypeAllowed { get; set; }
    }
}

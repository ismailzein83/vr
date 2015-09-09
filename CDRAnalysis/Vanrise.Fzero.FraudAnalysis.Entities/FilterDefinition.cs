using System;
using Vanrise.Fzero.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public enum CriteriaCompareOperator { GreaterThanorEqual, LessThanorEqual }

    public class FilterDefinition
    {
        public bool ExcludeHourly { get; set; }

        public int FilterId { get; set; }

        public CriteriaCompareOperator CompareOperator { get; set; }

        public string Description { get; set; }

        public string Label { get; set; }

        public Func<NumberProfile, Decimal> Expression;

        public float MinValue { get; set; }

        public float MaxValue { get; set; }

        public int DecimalPrecision { get; set; }

        public string ToolTip { get; set; }

        public OperatorTypeEnum OperatorTypeAllowed { get; set; }
    }
}

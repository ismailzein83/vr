using System;
using Vanrise.Fzero.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class FilterDefinitionInfo
    {
        public int FilterId { get; set; }

        public string Description { get; set; }

        public string Abbreviation { get; set; }

        public string Label { get; set; }

        public float MinValue { get; set; }

        public float MaxValue { get; set; }

        public int DecimalPrecision { get; set; }

        public bool ExcludeHourly { get; set; }

        public string ToolTip { get; set; }

        public OperatorTypeEnum OperatorTypeAllowed { get; set; }

        public string UpSign { get; set; }

        public string DownSign { get; set; }

    }
}

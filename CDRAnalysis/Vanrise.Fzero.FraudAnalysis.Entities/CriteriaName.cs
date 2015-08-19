using System;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class CriteriaName
    {
        public int FilterId { get; set; }

        public string Description { get; set; }

        public string Label { get; set; }

        public float MinValue { get; set; }

        public float MaxValue { get; set; }

        public int DecimalPrecision { get; set; }

    }
}

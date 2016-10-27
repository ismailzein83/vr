using System;


namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class NumberProfileQuery
    {
        public DateTime FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string AccountNumber { get; set; }

    }
}

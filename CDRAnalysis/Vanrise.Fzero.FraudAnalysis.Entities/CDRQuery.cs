using System;


namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class CDRQuery
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string MSISDN { get; set; }

    }
}

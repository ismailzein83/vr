using System;

namespace Vanrise.Fzero.CDRImport.Entities
{
    public class NormalCDRQuery
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string MSISDN { get; set; }

    }
}

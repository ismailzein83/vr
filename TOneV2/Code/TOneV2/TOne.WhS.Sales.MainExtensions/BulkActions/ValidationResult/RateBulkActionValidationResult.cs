using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.MainExtensions
{
    public class RateBulkActionValidationResult : BulkActionValidationResult
    {
        public List<InvalidZoneRate> EmptyRates { get; set; }

        public List<InvalidZoneRate> ZeroRates { get; set; }

        public List<InvalidZoneRate> NegativeRates { get; set; }

        public List<InvalidZoneRate> DuplicateRates { get; set; }

        public DateTime BED { get; set; }

        public RateBulkActionValidationResult()
        {
            base.ExcludedZoneIds = new List<long>();
            EmptyRates = new List<InvalidZoneRate>();
            ZeroRates = new List<InvalidZoneRate>();
            NegativeRates = new List<InvalidZoneRate>();
            DuplicateRates = new List<InvalidZoneRate>();
        }
    }

    public class InvalidZoneRate
    {
        public string ZoneName { get; set; }

        public decimal? CalculatedRate { get; set; }

        public RateBulkActionValidationResultType ValidationResultType { get; set; }
    }
}

using System;
using System.Collections.Generic;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.MainExtensions
{
    public class CustomerTargetMatchBulkActionCorrectedData: BulkActionCorrectedData
    {
        public List<IncludedZone> IncludedZones { get; set; }
    }


    public class IncludedZone
    {
        public long ZoneId { get; set; }
        public string ZoneName { get; set; }
        public decimal Rate { get; set; }
        public string TargetVolume { get; set; }
    }
}

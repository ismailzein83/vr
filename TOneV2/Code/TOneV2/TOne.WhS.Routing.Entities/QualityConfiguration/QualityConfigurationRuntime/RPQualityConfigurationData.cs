using System;
using System.Collections.Generic;

namespace TOne.WhS.Routing.Entities
{
    public class RPQualityConfigurationData : RouteRuleQualityConfigurationData
    {
        public long SaleZoneId { get; set; }
        public int SupplierId { get; set; }
    }

    //public class RPQualityConfigurationDataBatch
    //{
    //    public List<RPQualityConfigurationData> RPQualityConfigurationDataList { get; set; } 
    //}

    public struct SaleZoneSupplier
    {
        public long SaleZoneId { get; set; }
        public int SupplierId { get; set; }

        public override int GetHashCode()
        {
            return SaleZoneId.GetHashCode() + SupplierId.GetHashCode();
        }
    }
}
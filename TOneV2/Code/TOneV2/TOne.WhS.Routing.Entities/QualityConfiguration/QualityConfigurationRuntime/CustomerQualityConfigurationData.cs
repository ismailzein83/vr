using System;
using System.Collections.Generic;

namespace TOne.WhS.Routing.Entities
{
    public class CustomerRouteQualityConfigurationData
    {
        public Guid QualityConfigurationId { get; set; }
        public long SupplierZoneId { get; set; }
        public Decimal QualityData { get; set; }
    }

    public class CustomerRouteQualityConfigurationDataBatch
    {
        public List<CustomerRouteQualityConfigurationData> CustomerRouteQualityConfigurationDataList { get; set; }  
    }
}
using System;
using System.Collections.Generic;

namespace TOne.WhS.Routing.Entities
{
    public class CustomerRouteQualityConfigurationData : RouteRuleQualityConfigurationData
    {
        public long SupplierZoneId { get; set; }
    }

    //public class CustomerRouteQualityConfigurationDataBatch
    //{
    //    public List<CustomerRouteQualityConfigurationData> CustomerRouteQualityConfigurationDataList { get; set; }  
    //}
}
using System;
using System.Collections.Generic;

namespace TOne.WhS.RouteSync.Cataleya.Entities
{
    public interface IRouteInitializeContext
    {
        Dictionary<int, CustomerInfo> CustomersInfoByCustomer { get; }
    }
    public class RouteInitializeContext : IRouteInitializeContext
    {
        public Dictionary<int, CustomerInfo> CustomersInfoByCustomer { get; set; }
    }

    public interface IRouteFinalizeContext { }
    public class RouteFinalizeContext : IRouteFinalizeContext { }

    public interface IGetCarrierAccountsPreviousVersionNumbersContext
    {
        List<int> CarrierAccountIds { get; }
    }
    public class GetCarrierAccountsPreviousVersionNumbersContext : IGetCarrierAccountsPreviousVersionNumbersContext
    {
        public List<int> CarrierAccountIds { get; set; }
    }
}

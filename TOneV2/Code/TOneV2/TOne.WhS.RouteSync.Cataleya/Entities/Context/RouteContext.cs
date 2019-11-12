using System;
using System.Collections.Generic;

namespace TOne.WhS.RouteSync.Cataleya.Entities
{
    public interface IRouteInitializeContext
    {
        List<CustomerIdentification> CustomersIdentification { get; }
        List<CarrierAccountMapping> CarrierAccountsMapping { get; }
    }

    public class RouteInitializeContext : IRouteInitializeContext
    {
        public List<CustomerIdentification> CustomersIdentification { get; set; }
        public List<CarrierAccountMapping> CarrierAccountsMapping { get; set; }
    }

    public interface ICataleyaFinalizeContext
    {
        Dictionary<int, CarrierFinalizationData> CarrierFinalizationDataByCustomer { get; }
        List<String> RouteTableNames { get; }
    }
    public class CataleyaFinalizeContext : ICataleyaFinalizeContext
    {
        public Dictionary<int, CarrierFinalizationData> CarrierFinalizationDataByCustomer { get; set; }
        public List<String> RouteTableNames { get; set; }
    }

    public interface IGetCarrierAccountsPreviousVersionNumbersContext
    {
        List<int> CarrierAccountIds { get; }
    }

    public class GetCarrierAccountsPreviousVersionNumbersContext : IGetCarrierAccountsPreviousVersionNumbersContext
    {
        public List<int> CarrierAccountIds { get; set; }
    }
}

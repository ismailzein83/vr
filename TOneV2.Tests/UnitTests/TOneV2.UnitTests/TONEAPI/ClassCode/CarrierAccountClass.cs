using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TONEAPI.ClassCode
{
    public class CarrierAccountClass
    {
        public object ResultKey { get; set; }
        public IList<Datum> Data { get; set; }
        public int TotalCount { get; set; }
    }
    public class CarrierAccountSettings
    {
        public int ActivationStatus { get; set; }
        public int CurrencyId { get; set; }
        public string Mask { get; set; }
        public int NominalCapacity { get; set; }
    }

    public class SupplierSettings
    {
        public int RoutingStatus { get; set; }
    }

    public class CustomerSettings
    {
        public object DefaultRoutingProductId { get; set; }
        public int RoutingStatus { get; set; }
    }

    public class Entity
    {
        public int CarrierAccountId { get; set; }
        public string NameSuffix { get; set; }
        public int CarrierProfileId { get; set; }
        public int? SellingNumberPlanId { get; set; }
        public int AccountType { get; set; }
        public CarrierAccountSettings CarrierAccountSettings { get; set; }
        public SupplierSettings SupplierSettings { get; set; }
        public CustomerSettings CustomerSettings { get; set; }
        public object SourceId { get; set; }
    }

    public class Datum
    {
        public string CarrierProfileName { get; set; }
        public string CarrierAccountName { get; set; }
        public string AccountTypeDescription { get; set; }
        public string SellingNumberPlanName { get; set; }
        public Entity Entity { get; set; }
    }

  
}
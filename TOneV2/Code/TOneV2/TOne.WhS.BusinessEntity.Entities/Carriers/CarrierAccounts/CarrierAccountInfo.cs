using System;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class CarrierAccountInfo
    {
        public int CarrierAccountId { get; set; }
        public string Name { get; set; }
        public CarrierAccountType AccountType { get; set; }
        public ActivationStatus ActivationStatus { get; set; }
        public int? SellingNumberPlanId { get; set; }
        public int CurrencyId { get; set; }

    }
}

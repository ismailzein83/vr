using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class AccountManagerSettings : SettingData
    {
        public CarrierAccountFiltering CarrierAccountFiltering { get; set; }
    }
    public class CarrierAccountFiltering
    {
        public bool RatePlan { get; set; }
        public bool CustomerRoute { get; set; }
        public bool ProductRoute { get; set; }
        public bool Traffic { get; set; }
        public bool Billing { get; set; }
        public bool HideSupplierAmounts { get; set; }
    }
}

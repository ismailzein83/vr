using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class AccountManagerCarrier : CarrierAccountInfo
    {
        public bool IsCustomerAvailable { get; set; }
        public bool IsSupplierAvailable { get; set; }
        public CarrierAccountType CarrierType { get; set; }
    }
}

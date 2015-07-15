using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.BusinessEntity.Entities
{
    public class AccountManagerCarrier
    {
        public string CarrierAccountId { get; set; }
        public string Name { get; set; }
        public string NameSuffix { get; set; }
        public bool IsCustomerAvailable { get; set; }
        public bool IsSupplierAvailable { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class CarrierAccountToEdit
    {
        public int CarrierAccountId { get; set; }

        public string NameSuffix { get; set; }

        public CarrierAccountSettings CarrierAccountSettings { get; set; } 

        public CarrierAccountSupplierSettings SupplierSettings { get; set; }

        public CarrierAccountCustomerSettings CustomerSettings { get; set; }
    }
}

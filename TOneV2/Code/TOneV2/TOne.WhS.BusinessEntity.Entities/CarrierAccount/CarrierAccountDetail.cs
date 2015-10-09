using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
   public class CarrierAccountDetail
    {
        public int CarrierAccountId { get; set; }

        public string Name { get; set; }
        public string CarrierProfileName { get; set; }
       public string AccountTypeDescription{ get; set; }
        public CarrierAccountType AccountType { get; set; }

        public CarrierAccountSupplierSettings SupplierSettings { get; set; }

        public CarrierAccountCustomerSettings CustomerSettings { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.BusinessEntity.Entities
{
    class AssignedCarrierFromTempTable
    {
        public int CarrierAccountID { get; set; }
        public string Name { get; set; }
        public string NameSuffix { get; set; }
        public int AccountType { get; set; }
        public bool IsCustomerAssigned { get; set; }
        public bool IsSupplierAssigned { get; set; }
    }
}

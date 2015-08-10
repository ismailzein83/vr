using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.BusinessEntity.Entities
{
    public class AssignedCarrierFromTempTable
    {
        public string CarrierAccountID { get; set; }
        public string CarrierName { get; set; }
        public bool IsCustomerAssigned { get; set; }
        public bool IsSupplierAssigned { get; set; }
        public bool IsCustomerIndirect { get; set; }
        public bool IsSupplierIndirect { get; set; }
    }
}

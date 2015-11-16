using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class AssignedCarrierDetail
    {
        public AssignedCarrier Entity { get; set; }
        public string CarrierName { get; set; }
        public bool IsCustomerAssigned { get; set; }
        public bool IsSupplierAssigned { get; set; }
        public bool IsCustomerInDirect { get; set; }
        public bool IsSupplierInDirect { get; set; }

        public bool IsCustomerAvailable { get; set; }
        public bool IsSupplierAvailable { get; set; }


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SupplierZoneGroupContext:ISupplierZoneGroupContext
    {
        public SupplierZoneFilterSettings FilterSettings { get; set; }

        public IEnumerable<long> GetGroupSupplierZoneIds(SupplierZoneGroup supplierZoneGroup)
        {
            var allGroupZoneIds = supplierZoneGroup.GetSupplierZoneIds(this);
            return allGroupZoneIds;
        }
    }
}

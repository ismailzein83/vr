using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions.SupplierGroups
{
    public class SelectiveSupplierGroup : SupplierGroupSettings
    {
        public List<int> SupplierIds { get; set; }

        public override IEnumerable<int> GetSupplierIds(SupplierGroupContext context)
        {
            return this.SupplierIds;
        }
    }
}

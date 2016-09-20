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
        Guid _configId;
        public override Guid ConfigId { get { return _configId; } set { _configId = new Guid("0a724de7-08c2-4ed1-a3ef-4ac582432fe2"); } }
        public List<int> SupplierIds { get; set; }

        public override IEnumerable<int> GetSupplierIds(ISupplierGroupContext context)
        {
            return this.SupplierIds;
        }
    }
}

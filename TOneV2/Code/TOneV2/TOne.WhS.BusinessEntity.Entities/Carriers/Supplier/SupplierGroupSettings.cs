using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public abstract class SupplierGroupSettings
    {
        public virtual Guid ConfigId { get; set; }

        public abstract IEnumerable<int> GetSupplierIds(ISupplierGroupContext context);
    }

    public class SelectiveSuppliersSettings : SupplierGroupSettings
    {
        public List<int> SupplierIds { get; set; }

        public override IEnumerable<int> GetSupplierIds(ISupplierGroupContext context)
        {
            throw new NotImplementedException();
        }
    }
}

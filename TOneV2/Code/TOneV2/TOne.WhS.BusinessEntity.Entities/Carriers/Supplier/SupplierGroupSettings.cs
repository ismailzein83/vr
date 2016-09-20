using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public abstract class SupplierGroupSettings
    {
        public abstract Guid ConfigId { get; }

        public abstract IEnumerable<int> GetSupplierIds(ISupplierGroupContext context);
    }

    public class SelectiveSuppliersSettings : SupplierGroupSettings
    {
        public override Guid ConfigId { get { return new Guid("0a724de7-08c2-4ed1-a3ef-4ac582432fe2"); } }

        public List<int> SupplierIds { get; set; }

        public override IEnumerable<int> GetSupplierIds(ISupplierGroupContext context)
        {
            throw new NotImplementedException();
        }
    }
}

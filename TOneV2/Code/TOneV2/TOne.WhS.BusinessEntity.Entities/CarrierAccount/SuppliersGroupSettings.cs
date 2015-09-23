using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public abstract class SupplierGroupSettings
    {
        
    }

    public class SelectiveSuppliersSettings : SupplierGroupSettings
    {
        public List<int> SupplierIds { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public interface ISupplierGroupContext
    {
        SupplierFilterSettings FilterSettings { get; set; }

        IEnumerable<int> GetSupplierIds(SupplierGroupSettings group);
    }
}

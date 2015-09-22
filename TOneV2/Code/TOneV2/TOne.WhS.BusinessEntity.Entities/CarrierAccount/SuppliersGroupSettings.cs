using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public abstract class SuppliersGroupSettings
    {
        
    }

    public class SelectiveSuppliersSettings : SuppliersGroupSettings
    {
        public List<int> SupplierIds { get; set; }
    }

    //public class AllSuppliersSettings : SuppliersGroupSettings
    //{
        
    //}
}

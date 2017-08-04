using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public class SupplierTargetMatchQuery
    {
        //Add ActionParameters Class
        public SupplierTargetMatchFilter Filter { get; set; }
        public SupplierTargetSettings Settings { get; set; }

    }
}

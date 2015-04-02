using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.LCR.Entities
{
    public class BlockSuppliersRouteActionData
    {
        public List<BlockSupplierOption> BlockedOptions { get; set; }
    }

    public class BlockSupplierOption
    {
        public string SupplierId { get; set; }
    }
}

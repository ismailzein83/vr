using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.SupplierPriceList.Entities
{
    public class ReceivedPricelistQuery
    {
        public List<int> SupplierIds { get; set; }
        public List<int> Status { get; set; }
        public int Top { get; set; }
    }
}

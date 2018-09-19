using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public class CodeCompareQuery
    {
        public int threshold { get; set; }
        public int sellingNumberPlanId { get; set; }
        public IEnumerable<long> supplierIds { get; set; }
        public string codeStartWith { get; set; }
        public long? ZoneNameSupplierId { get; set; }
    }
}

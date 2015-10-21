using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Routing.Entities
{
    public class CodeMatches
    {
        public string Code { get; set; }

        public List<SaleCodeMatch> SaleCodeMatches { get; set; }

        public List<SupplierCodeMatch> SupplierCodeMatches { get; set; }

        public SupplierCodeMatchBySupplier SupplierCodeMatchesBySupplier { get; set; }
    }
}

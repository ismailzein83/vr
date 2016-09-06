using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Routing.Entities
{
    public class CodePrefix : ICode
    {
        public string Code { get; set; }

        public bool IsCodeDivided { get; set; }

        public int CodeCount { get; set; }
    }

    public class CodePrefixSaleCodes
    {
        public CodePrefix CodePrefix { get; set; }

        public IEnumerable<SaleCode> SaleCodes { get; set; }
    }

    public class CodePrefixSupplierCodes
    {
        public CodePrefix CodePrefix { get; set; }

        public IEnumerable<SupplierCode> SupplierCodes { get; set; }
    }
}
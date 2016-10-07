using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Business
{
    public class SupplierCodeIterator
    {
        public int SupplierId { get; set; }

        public CodeIterator<SupplierCode> CodeIterator { get; set; }

        public SupplierCodeMatch GetCodeMatch(string number)
        {
            SupplierCodeMatch supplierCodeMatch = null;
            SupplierCode matchSupplierCode = this.CodeIterator.GetLongestMatch(number);
            if (matchSupplierCode != null)
            {
                supplierCodeMatch = new SupplierCodeMatch
                {
                    SupplierId = this.SupplierId,
                    SupplierCode = matchSupplierCode.Code,
                    SupplierZoneId = matchSupplierCode.ZoneId,
                    SupplierCodeSourceId = matchSupplierCode.SourceId
                };
            }
            return supplierCodeMatch;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SupplierRateReadWithCache : ISupplierRateReader
    {
        public SupplierRateReadWithCache(DateTime effectiveOn)
        {

        }

        public SupplierRatesByZone GetSupplierRates(int supplierId)
        {
            throw new NotImplementedException();
        }
    }
}

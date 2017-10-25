using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public abstract class BaseSupplierRateQueryHandler
    {
        public DateTime EffectiveOn { get; set; }
        public bool IsSystemCurrency { get; set; }
        public abstract IEnumerable<SupplierRate> GetFilteredSupplierRates();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SaleEntityZoneRate
    {
        public SalePriceListOwnerType Source { get; set; }
        
        public SaleRate Rate { get; set; }

        public Dictionary<int, SaleRate> RatesByRateType { get; set; }

        // TODO: Remove this property and create a class to represent normal and other rates with their sources
        public Dictionary<int, SalePriceListOwnerType> SourcesByRateType { get; set; }

        public int EffectiveCurrencyId
        {
            get
            {
                ISaleEntityZoneRateManager manager = BEManagerFactory.GetManager<ISaleEntityZoneRateManager>();
                return manager.GetCurrencyId(this.Rate);
            }
        }
    }
}

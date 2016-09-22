using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Sales.Entities
{
    public class ApplyCalculatedRatesInput
    {
        public SalePriceListOwnerType OwnerType { get; set; }
        public int OwnerId { get; set; }
        public IEnumerable<CalculatedZoneRate> CalculatedRates { get; set; }
        public DateTime EffectiveOn { get; set; }
        public int CurrencyId { get; set; }
    }
}

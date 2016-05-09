using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SupplierZoneRate
    {
        public SupplierRate Rate { get; set; }

        public SupplierPriceList PriceList { get; set; }

        public int EffectiveCurrencyId
        {
            get
            {
                return this.Rate.CurrencyId.HasValue ? this.Rate.CurrencyId.Value : this.PriceList.CurrencyId;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class MoneyPackageItem : PackageItemSettings
    {
        public Decimal Amount { get; set; }

        public int? CurrencyId { get; set; }//System Currency if NULL
    }
}

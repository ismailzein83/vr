using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class MoneyPackageItem : PackageItemSettings
    {

        public override Guid ConfigId { get { return new Guid("e548dc54-6664-45e6-b5cf-9b84d046d782"); } }

        public Decimal Amount { get; set; }

        public int? CurrencyId { get; set; }//System Currency if NULL
    }
}

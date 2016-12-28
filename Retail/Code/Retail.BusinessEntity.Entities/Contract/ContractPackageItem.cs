using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class ContractPackageItem
    {
        public int PackageId { get; set; }

        public int CurrencyId { get; set; }

        public Decimal? InitialFee { get; set; }

        public RecurringPeriodSettings RecurringPeriod { get; set; }

        public Decimal? RecurringFee { get; set; }
    }
}

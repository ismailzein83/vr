using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class RecurringPrice
    {
        public RecurringPeriodSettings RecurringPeriod { get; set; }

        public Decimal? RecurringFee { get; set; }
    }
}

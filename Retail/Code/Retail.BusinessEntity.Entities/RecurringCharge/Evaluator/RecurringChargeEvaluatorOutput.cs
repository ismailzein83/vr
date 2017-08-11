using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class RecurringChargeEvaluatorOutput
    {
        public Guid ChargeableEntityId { get; set; }

        public Decimal Amount { get; set; }

        public int CurrencyId { get; set; }

        public DateTime ChargingStart { get; set; }

        public DateTime ChargingEnd { get; set; }
    }
}

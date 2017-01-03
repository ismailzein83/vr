using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class ApplicableRecurringCharge
    {
        public Guid RecurringChargeDefinitionId { get; set; }

        public AccountRecurringChargeEvaluator RecurringChargeEvaluator { get; set; }
    }
}

using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.RecurringChargeEvaluators
{
    public class OneTimeRecurringChargeEvaluatorDefinitionSettings : RecurringChargeEvaluatorDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { throw new NotImplementedException(); }
        }

        public Guid ChargeableEntityBEDefinitionId { get; set; }

        public Guid ChargeableEntityId { get; set; }
    }
}

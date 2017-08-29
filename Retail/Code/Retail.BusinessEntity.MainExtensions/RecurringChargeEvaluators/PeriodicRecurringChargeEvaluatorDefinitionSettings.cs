using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.RecurringChargeEvaluators
{
    public class PeriodicRecurringChargeEvaluatorDefinitionSettings : RecurringChargeEvaluatorDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("1F7F8131-E49E-4A1D-802A-0432BA92EBAB"); }
        }
        public override string RuntimeEditor { get { return "retail-be-packagesettings-recurcharge-evaluator-periodic"; } }

        public Guid ChargeableEntityBEDefinitionId { get; set; }

        public Guid ChargeableEntityId { get; set; }

        public List<Guid> PricingStatuses { get; set; }
        //public AccountCondition AccountCondition { get; set; }
    }
}

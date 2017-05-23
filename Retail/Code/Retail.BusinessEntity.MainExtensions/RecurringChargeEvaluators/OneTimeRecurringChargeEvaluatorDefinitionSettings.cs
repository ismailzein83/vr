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
            get { return new Guid("2C11E2C0-D54B-41DF-95FA-1FBCFD5C93B0"); }
        }
        public override string RuntimeEditor { get { return "retail-be-packagesettings-recurcharge-evaluator-onetime"; } }

        public Guid ChargeableEntityBEDefinitionId { get; set; }

        public Guid ChargeableEntityId { get; set; }
    }
}

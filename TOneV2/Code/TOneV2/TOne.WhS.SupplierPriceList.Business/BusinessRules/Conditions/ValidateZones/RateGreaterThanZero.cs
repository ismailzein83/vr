using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.SupplierPriceList.Business
{
    class RateGreaterThanZero : BusinessRuleCondition
    {
        public override string GetMessage(IRuleTarget target)
        {
            throw new NotImplementedException();
        }

        public override bool ShouldValidate(IRuleTarget target)
        {
            return target is ImportedRate;
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            ImportedRate importedRate = context.Target as ImportedRate;
            if (importedRate != null && importedRate.Rate <= 0)
            {
                context.Message = string.Format("Zone {0} Has Rate {1} < 0 ", importedRate.ZoneName, importedRate.Rate);
                return false;
            }
            return true;
        }
    }
}

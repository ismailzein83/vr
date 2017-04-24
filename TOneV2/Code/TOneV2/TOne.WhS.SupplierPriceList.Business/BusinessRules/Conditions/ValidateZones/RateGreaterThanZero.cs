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
            return target is ImportedDataByZone;
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            ImportedDataByZone importedZone = context.Target as ImportedDataByZone;
            if (importedZone != null && importedZone.ImportedNormalRates != null && importedZone.ImportedNormalRates.Count > 0)
            {
                ImportedRate importedRate = importedZone.ImportedNormalRates.First();
                if (importedRate.Rate <= 0)
                {
                    context.Message = string.Format("Zone {0} Has Rate <= 0 ", importedRate.ZoneName);
                    return false;
                }
            }
            return true;
        }
    }
}

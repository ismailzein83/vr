using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class MissingZonesCondition : BusinessRuleCondition
    {

        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as ImportedCode != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            ImportedCode code = context.Target as ImportedCode;
            var result = !(string.IsNullOrEmpty(code.ZoneName));

            if(result == false)
                context.Message = string.Format("Code {0} has a missing zone", code.Code);

            return result;
        }

        public override string GetMessage(IRuleTarget target)
        {
             return string.Format("Code {0} has a missing zone", (target as ImportedCode).Code);
        }

    }
}

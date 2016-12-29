using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.Sales.Business.BusinessRules
{
    public class MissingSellingProductDefaultServiceCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return target is DefaultData;
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            var defaultData = context.Target as DefaultData;

            if (defaultData.OwnerType == SalePriceListOwnerType.SellingProduct)
            {
                if (defaultData.CurrentServices == null && defaultData.DefaultServiceToAdd == null)
                {
                    context.Message = String.Format("Missing default service");
                    return false;
                }
            }

            return true;
        }

        public override string GetMessage(IRuleTarget target)
        {
            return String.Format("Missing default service");
        }
    }
}

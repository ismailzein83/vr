using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Business.BusinessRules
{
    public class SPDefaultRPDoesNotExistCondition : Vanrise.BusinessProcess.Entities.BusinessRuleCondition
    {
        public override bool ShouldValidate(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            return target is DefaultData;
        }

        public override bool Validate(Vanrise.BusinessProcess.Entities.IBusinessRuleConditionValidateContext context)
        {
            IRatePlanContext ratePlanContext = context.GetExtension<IRatePlanContext>();

            if (ratePlanContext.OwnerType == SalePriceListOwnerType.Customer)
                return true;

            var defaultData = context.Target as DefaultData;

            if (defaultData.DefaultRoutingProductToAdd == null && defaultData.CurrentDefaultRoutingProduct == null)
            {
                string sellingProductName = new SellingProductManager().GetSellingProductName(ratePlanContext.OwnerId);
                context.Message = string.Format("Selling Product '{0}' does not have a default Routing Product", sellingProductName);
                return false;
            }

            return true;
        }

        public override string GetMessage(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            return "Selling Product does not have a default Routing Product";
        }
    }
}

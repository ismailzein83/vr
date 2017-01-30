using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.NumberingPlan.Entities;

namespace Vanrise.NumberingPlan.Business
{
    public class ChangesInPastCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as CountryToProcess != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            ICPParametersContext cpContext = context.GetExtension<ICPParametersContext>();
            if (cpContext.EffectiveDate >= DateTime.Now.Date)
                return true;

            CountryToProcess country = context.Target as CountryToProcess;
            SaleZoneManager saleZoneManager = new SaleZoneManager();

            bool result = saleZoneManager.IsCountryEmpty(cpContext.SellingNumberPlanId, country.CountryId, DateTime.Now);

            if (result == false)
                context.Message = string.Format("Effective date can not be less than date of today");

            return result;
        }

        public override string GetMessage(IRuleTarget target)
        {
            return string.Format("Effective date can not be less than date of today");
        }
    }
}

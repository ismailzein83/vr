using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;
using Vanrise.Common.Business;

namespace TOne.WhS.Sales.Business.BusinessRules
{
	public class ChangedCountryEEDIsLessThanToday : Vanrise.BusinessProcess.Entities.BusinessRuleCondition
	{
		public override bool ShouldValidate(Vanrise.BusinessProcess.Entities.IRuleTarget target)
		{
			return (target is CustomerCountryToChange);
		}

		public override bool Validate(Vanrise.BusinessProcess.Entities.IBusinessRuleConditionValidateContext context)
		{
			var countryToChange = context.Target as CustomerCountryToChange;
			var result = (countryToChange.CloseEffectiveDate >= DateTime.Now.Date);

            if(result == false)
            {
                CountryManager countryManager = new CountryManager();
                context.Message = string.Format("EED '{0}' of Country '{1}' must be greater than or equal to today's date", countryToChange.CloseEffectiveDate, countryManager.GetCountryName(countryToChange.CountryId));
            }

            return result;
		}

		public override string GetMessage(Vanrise.BusinessProcess.Entities.IRuleTarget target)
		{
			var countryToChange = target as CustomerCountryToChange;
			string countryName = new Vanrise.Common.Business.CountryManager().GetCountryName(countryToChange.CountryId);
			return string.Format("EED '{0}' of Country '{1}' must be greater than or equal to today's date", countryToChange.CloseEffectiveDate, countryName);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Business.BusinessRules
{
	public class NewCountryBEDIsLessThanRetroactiveDateCondition : Vanrise.BusinessProcess.Entities.BusinessRuleCondition
	{
		public override bool ShouldValidate(Vanrise.BusinessProcess.Entities.IRuleTarget target)
		{
			return (target is CustomerCountryToAdd);
		}

		public override bool Validate(Vanrise.BusinessProcess.Entities.IBusinessRuleConditionValidateContext context)
		{
			var countryToAdd = context.Target as CustomerCountryToAdd;
			IRatePlanContext ratePlanContext = context.GetExtension<IRatePlanContext>();
			return (countryToAdd.BED >= ratePlanContext.RetroactiveDate);
		}

		public override string GetMessage(Vanrise.BusinessProcess.Entities.IRuleTarget target)
		{
			var countryToAdd = target as CustomerCountryToAdd;
			string countryName = new Vanrise.Common.Business.CountryManager().GetCountryName(countryToAdd.CountryId);
			return string.Format("BED '{0}' of Country '{1}' must be greater than or equal to the Retroactive date", countryToAdd.BED, countryName);
		}
	}
}

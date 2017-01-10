using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Business.BusinessRules
{
	public class ZoneCountryIsEndedAndZoneIsChangedCondition : Vanrise.BusinessProcess.Entities.BusinessRuleCondition
	{
		public override bool ShouldValidate(Vanrise.BusinessProcess.Entities.IRuleTarget target)
		{
			return (target is DataByZone);
		}

		public override bool Validate(Vanrise.BusinessProcess.Entities.IBusinessRuleConditionValidateContext context)
		{
			DataByZone zone = context.Target as DataByZone;

			if (zone.IsCountryEnded)
			{
				IEnumerable<string> actionMessages = BusinessRuleUtilities.GetZoneActionMessages(zone);
				if (actionMessages != null && actionMessages.Count() > 0)
				{
					string endedCountryName = new Vanrise.Common.Business.CountryManager().GetCountryName(zone.CountryId);
					context.Message = string.Format("Zone '{0}' of ended country '{1}' has the following actions: {2}", zone.ZoneName, endedCountryName, string.Join(" | ", actionMessages));
					return false;
				}
			}

			return true;
		}

		public override string GetMessage(Vanrise.BusinessProcess.Entities.IRuleTarget target)
		{
			throw new NotImplementedException();
		}
	}
}

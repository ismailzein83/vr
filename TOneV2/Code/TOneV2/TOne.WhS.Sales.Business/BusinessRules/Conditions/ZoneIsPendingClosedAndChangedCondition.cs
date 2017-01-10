using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Business.BusinessRules
{
	public class ZoneIsPendingClosedAndChangedCondition : Vanrise.BusinessProcess.Entities.BusinessRuleCondition
	{
		public override bool ShouldValidate(Vanrise.BusinessProcess.Entities.IRuleTarget target)
		{
			return target is DataByZone;
		}

		public override bool Validate(Vanrise.BusinessProcess.Entities.IBusinessRuleConditionValidateContext context)
		{
			DataByZone zone = context.Target as DataByZone;

			if (zone.EED.HasValue)
			{
				IEnumerable<string> actionMessages = BusinessRuleUtilities.GetZoneActionMessages(zone);
				if (actionMessages != null && actionMessages.Count() > 0)
				{
					context.Message = string.Format("Pending closed zone '{0}' has the following actions: {1}", zone.ZoneName, string.Join(" | ", actionMessages));
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

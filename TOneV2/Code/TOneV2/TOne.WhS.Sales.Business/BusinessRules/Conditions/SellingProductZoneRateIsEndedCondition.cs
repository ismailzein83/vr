using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Business.BusinessRules
{
	public class SellingProductZoneRateIsEndedCondition : Vanrise.BusinessProcess.Entities.BusinessRuleCondition
	{
		public override bool ShouldValidate(Vanrise.BusinessProcess.Entities.IRuleTarget target)
		{
			return (target is DataByZone);
		}

		public override bool Validate(Vanrise.BusinessProcess.Entities.IBusinessRuleConditionValidateContext context)
		{
			IRatePlanContext ratePlanContext = context.GetExtension<IRatePlanContext>();

			if (ratePlanContext.OwnerType == TOne.WhS.BusinessEntity.Entities.SalePriceListOwnerType.Customer)
				return true;

			var zoneData = context.Target as DataByZone;

			if (zoneData.NormalRateToClose != null)
				return false;

			if (zoneData.OtherRatesToClose != null && zoneData.OtherRatesToClose.Count > 0)
				return false;

			if (zoneData.NormalRateToChange != null && zoneData.NormalRateToChange.EED.HasValue)
				return false;

			if (zoneData.OtherRatesToChange != null)
			{
				foreach (RateToChange otherRateToChange in zoneData.OtherRatesToChange)
				{
					if (otherRateToChange.EED.HasValue)
						return false;
				}
			}

			return true;
		}

		public override string GetMessage(Vanrise.BusinessProcess.Entities.IRuleTarget target)
		{
			var zoneData = target as DataByZone;
			return string.Format("Selling Product Zone '{0}' either has a New Rate with an EED or a Closed Existing Rate", zoneData.ZoneName);
		}
	}
}

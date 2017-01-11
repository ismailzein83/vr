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
	public class CustomerZoneRateHasInvalidEEDCondition : Vanrise.BusinessProcess.Entities.BusinessRuleCondition
	{
		public override bool ShouldValidate(Vanrise.BusinessProcess.Entities.IRuleTarget target)
		{
			return target is DataByZone;
		}

		public override bool Validate(Vanrise.BusinessProcess.Entities.IBusinessRuleConditionValidateContext context)
		{
			IRatePlanContext ratePlanContext = context.GetExtension<IRatePlanContext>();

			if (ratePlanContext.OwnerType == SalePriceListOwnerType.SellingProduct)
				return true;

			var zoneData = context.Target as DataByZone;

			var customerSellingProductManager = new CustomerSellingProductManager();
			int? sellingProductId = customerSellingProductManager.GetEffectiveSellingProductId(ratePlanContext.OwnerId, ratePlanContext.EffectiveDate, false);
			if (!sellingProductId.HasValue)
			{
				throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("Customer '{0}' is not assigned to a selling product on '{1}'", ratePlanContext.OwnerId, UtilitiesManager.GetDateTimeAsString(ratePlanContext.EffectiveDate)));
			}

			SaleEntityZoneRate sellingProductRate = ratePlanContext.FutureRateLocator.GetSellingProductZoneRate(sellingProductId.Value, zoneData.ZoneId);
			
			if (sellingProductRate == null || sellingProductRate.Rate == null)
			{
				// SellingProductNormalRateDoesNotExistCondition handles this case
				return true;
			}

			if (zoneData.NormalRateToClose != null)
			{
				if (zoneData.NormalRateToClose.CloseEffectiveDate < sellingProductRate.Rate.BED)
				{
					context.Message = string.Format("Normal rate of zone '{0}' is closed on a date '{1}' that's different from the BED '{2}' of its inherited normal rate", zoneData.ZoneName, UtilitiesManager.GetDateTimeAsString(zoneData.NormalRateToClose.CloseEffectiveDate), UtilitiesManager.GetDateTimeAsString(sellingProductRate.Rate.BED));
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

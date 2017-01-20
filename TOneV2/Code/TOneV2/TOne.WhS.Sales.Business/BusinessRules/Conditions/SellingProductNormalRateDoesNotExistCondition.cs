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
	public class SellingProductNormalRateDoesNotExistCondition : Vanrise.BusinessProcess.Entities.BusinessRuleCondition
	{
		public override bool ShouldValidate(Vanrise.BusinessProcess.Entities.IRuleTarget target)
		{
			return (target is DataByZone);
		}

		public override bool Validate(Vanrise.BusinessProcess.Entities.IBusinessRuleConditionValidateContext context)
		{
			IRatePlanContext ratePlanContext = context.GetExtension<IRatePlanContext>();

			if (ratePlanContext.OwnerType == SalePriceListOwnerType.SellingProduct)
				return true;

			var zoneData = context.Target as DataByZone;

			if (zoneData.NormalRateToClose != null)
			{
				var customerSellingProductManager = new CustomerSellingProductManager();
				int? sellingProductId = customerSellingProductManager.GetEffectiveSellingProductId(ratePlanContext.OwnerId, ratePlanContext.EffectiveDate, false);
				if (!sellingProductId.HasValue)
				{
					throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("Customer '{0}' is not assigned to a selling product on '{1}'", ratePlanContext.OwnerId, UtilitiesManager.GetDateTimeAsString(ratePlanContext.EffectiveDate)));
				}

				SaleEntityZoneRate sellingProductRate = ratePlanContext.FutureRateLocator.GetSellingProductZoneRate(sellingProductId.Value, zoneData.ZoneId);

				if (sellingProductRate == null || sellingProductRate.Rate == null)
				{
					context.Message = string.Format("Selling product does not have a normal rate for zone '{0}'", zoneData.ZoneName);
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

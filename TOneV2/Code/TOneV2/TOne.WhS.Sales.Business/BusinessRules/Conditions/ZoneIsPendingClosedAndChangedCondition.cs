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
			return (target is DataByZone);
		}

		public override bool Validate(Vanrise.BusinessProcess.Entities.IBusinessRuleConditionValidateContext context)
		{
			DataByZone zone = context.Target as DataByZone;

			if (zone.EED.HasValue)
			{
				if (zone.NormalRateToChange != null)
				{
					context.Message = string.Format("A New Normal Rate is defined for Pending Closed Zone '{0}'", zone.ZoneName);
					return false;
				}

				if (zone.NormalRateToClose != null)
				{
					context.Message = string.Format("An Existing Normal Rate is closed for Pending Closed Zone '{0}'", zone.ZoneName);
					return false;
				}

				var rateTypeManager = new Vanrise.Common.Business.RateTypeManager();

				if (zone.OtherRatesToChange != null && zone.OtherRatesToChange.Count > 0)
				{
					var rateTypeNames = new List<string>();
					foreach (RateToChange otherRateToChange in zone.OtherRatesToChange)
						AddRateTypeName(rateTypeNames, otherRateToChange.RateTypeId.Value, rateTypeManager);

					context.Message = string.Format("Other Rates '{0}' are defined for Pending Closed Zone '{1}'", string.Join(",", rateTypeNames), zone.ZoneName);
					return false;
				}

				if (zone.OtherRatesToClose != null && zone.OtherRatesToClose.Count > 0)
				{
					var rateTypeNames = new List<string>();
					foreach (RateToClose otherRateToClose in zone.OtherRatesToClose)
						AddRateTypeName(rateTypeNames, otherRateToClose.RateTypeId.Value, rateTypeManager);

					context.Message = string.Format("Other Rates '{0}' are closed for Pending Closed Zone '{1}'", string.Join(",", rateTypeNames), zone.ZoneName);
					return false;
				}

				if (zone.SaleZoneRoutingProductToAdd != null)
				{
					context.Message = string.Format("A New Routing Product is defined for Pending Closed Zone '{0}'", zone.ZoneName);
					return false;
				}

				if (zone.SaleZoneRoutingProductToClose != null)
				{
					context.Message = string.Format("An Existing Routing Product is closed for Pending Closed Zone '{0}'", zone.ZoneName);
					return false;
				}
			}
			
			return true;
		}

		public override string GetMessage(Vanrise.BusinessProcess.Entities.IRuleTarget target)
		{
			throw new NotImplementedException();
		}

		#region Private Methods

		private void AddRateTypeName(List<string> rateTypeNames, int rateTypeId, Vanrise.Common.Business.RateTypeManager rateTypeManager)
		{
			string rateTypeName = rateTypeManager.GetRateTypeName(rateTypeId);
			if (rateTypeName == null)
				throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("Name of Rate Type '{0}' was not found", rateTypeId));
			rateTypeNames.Add(rateTypeName);
		}
		
		#endregion
	}
}

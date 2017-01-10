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
			var errorMessages = new List<string>();

			if (zone.IsCountryEnded)
			{
				var rateTypeManager = new Vanrise.Common.Business.RateTypeManager();

				if (zone.NormalRateToChange != null)
					errorMessages.Add("A new normal rate is defined");

				if (zone.NormalRateToClose != null)
					errorMessages.Add("An existing normal rate is closed");

				if (zone.OtherRatesToChange != null && zone.OtherRatesToChange.Count > 0)
				{
					var rateTypeNames = new List<string>();
					foreach (RateToChange otherRateToChange in zone.OtherRatesToChange)
						AddRateTypeName(rateTypeNames, otherRateToChange.RateTypeId.Value, rateTypeManager);
					errorMessages.Add(string.Format("Other rates '{0}' are defined", string.Join(",", rateTypeNames)));
				}

				if (zone.OtherRatesToClose != null && zone.OtherRatesToClose.Count > 0)
				{
					var rateTypeNames = new List<string>();
					foreach (RateToClose otherRateToClose in zone.OtherRatesToClose)
						AddRateTypeName(rateTypeNames, otherRateToClose.RateTypeId.Value, rateTypeManager);
					errorMessages.Add(string.Format("Existing other rates '{0}' are closed", string.Join(",", rateTypeNames)));
				}

				if (zone.SaleZoneRoutingProductToAdd != null)
					errorMessages.Add("A new routing product is defined");

				if (zone.SaleZoneRoutingProductToClose != null)
					errorMessages.Add("An existing routing product is closed");
			}

			if (errorMessages.Count > 0)
			{
				string endedCountryName = new Vanrise.Common.Business.CountryManager().GetCountryName(zone.CountryId);
				context.Message = string.Format("Zone '{0}' of ended country '{1}' has the following actions: {2}", zone.ZoneName, endedCountryName, string.Join(" | ", errorMessages));
				return false;
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
			if (rateTypeName != null)
				rateTypeNames.Add(rateTypeName);
		}

		#endregion
	}
}

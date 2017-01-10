using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Business.BusinessRules
{
	public class BusinessRuleUtilities
	{
		public static IEnumerable<string> GetZoneActionMessages(DataByZone zone)
		{
			var actionMessages = new List<string>();
			var rateTypeManager = new Vanrise.Common.Business.RateTypeManager();

			if (zone.NormalRateToChange != null)
				actionMessages.Add("A new normal rate is defined");

			if (zone.NormalRateToClose != null)
				actionMessages.Add("An existing normal rate is closed");

			if (zone.OtherRatesToChange != null && zone.OtherRatesToChange.Count > 0)
			{
				var rateTypeNames = new List<string>();
				foreach (RateToChange otherRateToChange in zone.OtherRatesToChange)
					AddRateTypeName(rateTypeNames, otherRateToChange.RateTypeId.Value, rateTypeManager);
				actionMessages.Add(string.Format("Other rates '{0}' are defined", string.Join(",", rateTypeNames)));
			}

			if (zone.OtherRatesToClose != null && zone.OtherRatesToClose.Count > 0)
			{
				var rateTypeNames = new List<string>();
				foreach (RateToClose otherRateToClose in zone.OtherRatesToClose)
					AddRateTypeName(rateTypeNames, otherRateToClose.RateTypeId.Value, rateTypeManager);
				actionMessages.Add(string.Format("Existing other rates '{0}' are closed", string.Join(",", rateTypeNames)));
			}

			if (zone.SaleZoneRoutingProductToAdd != null)
				actionMessages.Add("A new routing product is defined");

			if (zone.SaleZoneRoutingProductToClose != null)
				actionMessages.Add("An existing routing product is closed");

			return actionMessages;
		}

		#region Private Methods

		private static void AddRateTypeName(List<string> rateTypeNames, int rateTypeId, Vanrise.Common.Business.RateTypeManager rateTypeManager)
		{
			string rateTypeName = rateTypeManager.GetRateTypeName(rateTypeId);
			if (rateTypeName != null)
				rateTypeNames.Add(rateTypeName);
		}

		#endregion
	}
}

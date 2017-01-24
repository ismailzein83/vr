using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;

namespace TOne.WhS.Sales.Business
{
	public class UtilitiesManager
	{
		public static DateTime? GetMaxDate(IEnumerable<DateTime?> dates)
		{
			int count;
			DateTime? maxDate = GetFirstDate(dates, out count);

			if (count == 1)
				return maxDate;

			for (int i = 1; i < count; i++)
			{
				DateTime? date = dates.ElementAt(i);
				if (date.HasValue)
				{
					if (!maxDate.HasValue)
						maxDate = date;
					else if (date.Value > maxDate.Value)
						maxDate = date;
				}
			}

			return maxDate;
		}

		public static DateTime? GetMinDate(IEnumerable<DateTime?> dates)
		{
			int count;
			DateTime? minDate = GetFirstDate(dates, out count);

			if (count == 1)
				return minDate;

			for (int i = 1; i < count; i++)
			{
				DateTime? date = dates.ElementAt(i);
				if (date.HasValue)
				{
					if (!minDate.HasValue)
						minDate = date;
					else if (date.Value < minDate.Value)
						minDate = date;
				}
			}

			return minDate;
		}

		public static string GetDateTimeAsString(DateTime dateTime)
		{
			return dateTime.ToShortDateString();
		}

		public static bool IsActionApplicableToZone(IsActionApplicableToZoneInput context)
		{
			ZoneChanges zoneDraft = null;

			if (context.Draft != null && context.Draft.ZoneChanges != null)
				zoneDraft = context.Draft.ZoneChanges.FindRecord(x => x.ZoneId == context.SaleZone.SaleZoneId);

			var actionApplicableToZoneContext = new ActionApplicableToZoneContext(context.GetSellingProductZoneRate, context.GetCustomerZoneRate)
			{
				OwnerType = context.OwnerType,
				OwnerId = context.OwnerId,
				SaleZone = context.SaleZone,
				ZoneDraft = zoneDraft
			};

			return context.BulkAction.IsApplicableToZone(actionApplicableToZoneContext);
		}

		public static Dictionary<int, DateTime> GetDatesByCountry(int customerId, DateTime? effectiveOn, bool isEffectiveInFuture)
		{
			var datesByCountry = new Dictionary<int, DateTime>();

			IEnumerable<CustomerCountry2> customerCountries = new CustomerCountryManager().GetCustomerCountries(customerId, effectiveOn, false);
			if (customerCountries != null)
			{
				foreach (CustomerCountry2 customerCountry in customerCountries)
					if (!datesByCountry.ContainsKey(customerCountry.CountryId))
						datesByCountry.Add(customerCountry.CountryId, customerCountry.BED);
			}

			Changes draft = new RatePlanDraftManager().GetDraft(SalePriceListOwnerType.Customer, customerId);
			if (draft != null && draft.CountryChanges != null && draft.CountryChanges.NewCountries != null)
			{
				foreach (DraftNewCountry newCountry in draft.CountryChanges.NewCountries)
				{
					if (!datesByCountry.ContainsKey(newCountry.CountryId))
						datesByCountry.Add(newCountry.CountryId, newCountry.BED);
				}
			}

			return datesByCountry;
		}

		public static bool IsCustomerZoneCountryApplicable(int countryId, DateTime rateBED, Dictionary<int, DateTime> datesByCountry)
		{
			DateTime soldOn;
			
			if (!datesByCountry.TryGetValue(countryId, out soldOn))
				return false;

			if (soldOn > rateBED)
				return false;
			
			return true;
		}

		#region Private Methods
		private static DateTime? GetFirstDate(IEnumerable<DateTime?> dates, out int count)
		{
			if (dates == null)
				throw new ArgumentNullException("dates");

			count = dates.Count();
			if (count == 0)
				throw new ArgumentException("dates.Count = 0");

			return dates.ElementAt(0);
		}
		#endregion
	}

	public class IsActionApplicableToZoneInput
	{
		public SalePriceListOwnerType OwnerType { get; set; }

		public int OwnerId { get; set; }

		public SaleZone SaleZone { get; set; }

		public BulkActionType BulkAction { get; set; }

		public Changes Draft { get; set; }

		public Func<int, long, SaleEntityZoneRate> GetSellingProductZoneRate { get; set; }

		public Func<int, int, long, SaleEntityZoneRate> GetCustomerZoneRate { get; set; }
	}
}

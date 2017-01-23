using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
				zoneDraft = context.Draft.ZoneChanges.FindRecord(x => x.ZoneId == context.ZoneId);

			var actionApplicableToZoneContext = new ActionApplicableToZoneContext(context.GetSellingProductZoneRate, context.GetCustomerZoneRate)
			{
				OwnerType = context.OwnerType,
				OwnerId = context.OwnerId,
				ZoneId = context.ZoneId,
				ZoneDraft = zoneDraft
			};

			return context.BulkAction.IsApplicableToZone(actionApplicableToZoneContext);
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

		#region Public Classes

		public class IsActionApplicableToZoneInput
		{
			public SalePriceListOwnerType OwnerType { get; set; }

			public int OwnerId { get; set; }

			public long ZoneId { get; set; }

			public BulkActionType BulkAction { get; set; }

			public Changes Draft { get; set; }

			public Func<int, long, SaleEntityZoneRate> GetSellingProductZoneRate { get; set; }

			public Func<int, int, long, SaleEntityZoneRate> GetCustomerZoneRate { get; set; }
		}

		#endregion
	}
}

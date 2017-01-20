﻿using System;
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

		public static bool IsActionApplicableToZone(BulkActionType bulkAction, long zoneId, Changes draftData)
		{
			ZoneChanges zoneDraft = null;

			if (draftData != null && draftData.ZoneChanges != null)
				zoneDraft = draftData.ZoneChanges.FindRecord(x => x.ZoneId == zoneId);

			var actionApplicableToZoneContext = new ActionApplicableToZoneContext()
			{
				ZoneId = zoneId,
				ZoneDraft = zoneDraft
			};

			return bulkAction.IsApplicableToZone(actionApplicableToZoneContext);
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
}

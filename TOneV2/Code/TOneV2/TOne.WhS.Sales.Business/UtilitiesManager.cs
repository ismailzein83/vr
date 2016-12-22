using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

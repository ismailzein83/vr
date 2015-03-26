using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BI.Entities;

namespace TOne.BI.Data.SQL
{
    public class DestinationDataManager : BaseDataManager, IDestinationDataManager
    {
        public IEnumerable<ZoneValue> GetTopZonesByDuration(TimeDimensionType timeDimensionType, DateTime fromDate, DateTime toDate, int topCount)
        {
            List<ZoneValue> rslt = new List<ZoneValue>();
            string columnsPart = BuildQueryColumnsPart(MeasureColumns.DURATION_IN_MINUTES);
            string rowsPart = BuildQueryTopRowsPart(MeasureColumns.DURATION_IN_MINUTES, topCount, SaleZoneColumns.ZONE_ID, SaleZoneColumns.ZONE_NAME);
            string filtersPart = GetDateFilter(fromDate, toDate);
            string query = BuildQuery(columnsPart, rowsPart, filtersPart);

            ExecuteReaderMDX(query, (reader) =>
            {
                while (reader.Read())
                {
                    ZoneValue zoneValue = new ZoneValue
                    {
                        ZoneId = Convert.ToInt32(reader[GetRowColumnToRead(SaleZoneColumns.ZONE_ID)]),
                        ZoneName = reader[GetRowColumnToRead(SaleZoneColumns.ZONE_NAME)] as string,
                        Value = Convert.ToDecimal(reader[MeasureColumns.DURATION_IN_MINUTES])
                    };
                    rslt.Add(zoneValue);
                }
            });
            return rslt;
        }
    }
}

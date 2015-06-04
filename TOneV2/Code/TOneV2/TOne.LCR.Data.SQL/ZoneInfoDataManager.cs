using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;
using Vanrise.Data.SQL;

namespace TOne.LCR.Data.SQL
{
    public class ZoneInfoDataManager : RoutingDataManager, IZoneInfoDataManager
    {
        public Object PrepareZoneInfosForDBApply(List<ZoneInfo> zoneInfos)
        {
            string filePath = GetFilePathForBulkInsert();
            using (System.IO.StreamWriter wr = new System.IO.StreamWriter(filePath))
            {
                foreach (var z in zoneInfos)
                {
                    wr.WriteLine(String.Format("{0}^{1}", z.ZoneId, z.Name));
                }
                wr.Close();
            }

            return new BulkInsertInfo
            {
                TableName = "ZoneInfo",
                DataFilePath = filePath,
                TabLock = true,
                FieldSeparator = '^'
            };
        }

        public void ApplyZoneInfosToDB(Object preparedZoneInfos)
        {
            InsertBulkToTable(preparedZoneInfos as BulkInsertInfo);
        }
    }
}

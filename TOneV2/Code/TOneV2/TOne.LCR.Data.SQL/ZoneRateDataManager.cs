using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using TOne.BusinessEntity.Entities;
using TOne.Data.SQL;
using Vanrise.Data.SQL;

namespace TOne.LCR.Data.SQL
{
    public class ZoneRateDataManager : RoutingDataManager, IZoneRateDataManager
    {
        public void InsertZoneRates(bool isSupplierZoneRates, List<ZoneRate> zoneRates)
        {
            string filePath = GetFilePathForBulkInsert();
            using (System.IO.StreamWriter wr = new System.IO.StreamWriter(filePath))
            {
                foreach (var zr in zoneRates)
                {
                    wr.WriteLine(String.Format("{0}^{1}^{2}^{3}^{4}^{5}", zr.RateId, zr.PriceListId, zr.ZoneId, zr.CarrierAccountId, zr.Rate, zr.ServicesFlag));
                }
                wr.Close();
            }
            var bulkInsertInfo = new BulkInsertInfo
            {
                TableName = String.Format("{0}ZoneRate", isSupplierZoneRates ? "Supplier" : "Customer"),
                DataFilePath = filePath,
                TabLock = true,
                FieldSeparator = '^'
            };
            InsertBulkToTable(bulkInsertInfo);
        }

        #region Queries

        #endregion
    }
}
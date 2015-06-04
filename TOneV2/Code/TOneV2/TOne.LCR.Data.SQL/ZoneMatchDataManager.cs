using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOne.Data.SQL;
using TOne.LCR.Entities;
using Vanrise.Data.SQL;

namespace TOne.LCR.Data.SQL
{
    public class ZoneMatchDataManager : RoutingDataManager, IZoneMatchDataManager
    {
        public void UpdateAll()
        {
            ExecuteNonQueryText(query_UpdateAll, null);
        }

        public void CreateIndexesOnTable()
        {
            ExecuteNonQueryText(query_CreateIndexesOnTable, null);
        }

        public void ApplyZoneMatchesToTempTable(SaleZoneMatches preparedZoneMatches)
        {
            InsertBulkToTable(PrepareZoneMatchesForDBApply(preparedZoneMatches) as BaseBulkInsertInfo);
        }

        #region Private Methods

        private object PrepareZoneMatchesForDBApply(SaleZoneMatches zoneMatches)
        {
            string filePath = GetFilePathForBulkInsert();
            using (System.IO.StreamWriter wr = new System.IO.StreamWriter(filePath))
            {
                foreach (var z in zoneMatches)
                {
                    foreach (var supplierZoneInfo in z.Value)
                    {
                        wr.WriteLine(String.Format("{0}^{1}^{2}^{3}", z.Key, supplierZoneInfo.Key,supplierZoneInfo.Value.SupplierId, supplierZoneInfo.Value.IsCodeGroup ? 1 : 0));
                    }

                }
                wr.Close();
            }

            return new BulkInsertInfo
            {
                TableName = "TempZoneMatch",
                DataFilePath = filePath,
                TabLock = true,
                FieldSeparator = '^'
            };
        }

        #endregion

        #region Queries

        const string query_UpdateAll = @" 
                                        SET NOCOUNT ON;
                                        SELECT DISTINCT zm.OurZoneId INTO #SaleZoneMatchedCodeGroup FROM TempZoneMatch zm WHERE zm.IsCodeGroup = 1

                                        INSERT INTO ZoneMatch WITH (TABLOCK)
                                                    ([OurZoneID]
                                                    ,[SupplierZoneID]
                                                    ,[SupplierID]
                                                    ,IsCodeGroup)
                                        SELECT DISTINCT tzm.OurZoneID, tzm.SupplierZoneID, tzm.SupplierID, tzm.IsCodeGroup
                                        FROM  TempZoneMatch tzm
                                        WHERE tzm.OurZoneId IN( SELECT * FROM #SaleZoneMatchedCodeGroup) AND tzm.IsCodeGroup = 1
							                                 
                                        INSERT INTO ZoneMatch WITH (TABLOCK)
                                                    ([OurZoneID]
                                                    ,[SupplierZoneID]
                                                    ,[SupplierID]
                                                    ,IsCodeGroup)
                                        SELECT DISTINCT tzm.OurZoneID, tzm.SupplierZoneID, tzm.SupplierID, tzm.IsCodeGroup
                                        FROM  TempZoneMatch tzm
                                        WHERE tzm.OurZoneId not IN( SELECT * FROM #SaleZoneMatchedCodeGroup) AND tzm.IsCodeGroup = 0
                                        ORDER BY tzm.OurZoneId";


        const string query_CreateIndexesOnTable = @"ALTER TABLE [ZoneMatch] ADD PRIMARY KEY CLUSTERED 
                                                        (
	                                                        [OurZoneID] ASC,
	                                                        [SupplierZoneID] ASC
                                                        )";


        #endregion



    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.WhS.SupplierPriceList.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.SupplierPriceList.Data.SQL
{
    public class ImportPriceListDataManager : BaseTOneDataManager, IImportPriceListDataManager
    {
        public ImportPriceListDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        public void InsertPriceListObject(List<Zone> supplierZones,List<Code> codesToBeDeleted)
        {
           
            CreateTempTableForSupplierZones(tempZoneTable);
            CreateTempTableForSupplierCodes(tempCodeTable);
            CreateTempTableForSupplierRates(tempRateTable);


            object dbApplyStreamZones = InitialiazeZonesStreamForDBApply();
            object dbApplyStreamCodes = InitialiazeCodesStreamForDBApply();
            object dbApplyStreamRates = InitialiazeRatesStreamForDBApply();
            foreach (Zone zone in supplierZones)
            {
                WriteRecordToZonesStream(zone, dbApplyStreamZones);
                foreach (Code code in zone.Codes)
                {
                    WriteRecordToCodesStream(code, dbApplyStreamCodes);
                }
                foreach (Rate rate in zone.Rates)
                {
                    WriteRecordToRatesStream(rate, dbApplyStreamRates);
                }
            }
            if (codesToBeDeleted != null && codesToBeDeleted.Count > 0)
            {
                foreach (Code code in codesToBeDeleted)
                {
                    WriteRecordToCodesStream(code, dbApplyStreamCodes);
                }
            }
       

            object prepareToApplyZones = FinishDBApplyStream(dbApplyStreamZones, tempZoneTable);
            object prepareToApplyCodes = FinishDBApplyStream(dbApplyStreamCodes, tempCodeTable);
            object prepareToApplyRates = FinishDBApplyStream(dbApplyStreamRates, tempRateTable);
            ApplySupplierZonesForTempDB(prepareToApplyZones);
            ApplySupplierZonesForTempDB(prepareToApplyCodes);
            ApplySupplierZonesForTempDB(prepareToApplyRates);

            MergingDataAndFinalizing();
        }
        private void CreateTempTableForSupplierZones(string tempTable)
        {
            StringBuilder queryBuilder = new StringBuilder(@"
                        IF NOT OBJECT_ID('#TempTable#', N'U') IS NOT NULL
	                        BEGIN
                                 CREATE TABLE #TempTable# (              
	                             [ID] [bigint] NOT NULL,
	                             [Name] [nvarchar](255) NOT NULL,
	                             [SupplierID] [int] NOT NULL,
	                             [BED] [datetime] NOT NULL,
	                             [EED] [datetime] NULL,
                                 [IsUpdated] [Bit] Null)
                             END ");
            queryBuilder.Replace("#TempTable#", tempTable);
            ExecuteNonQueryText(queryBuilder.ToString(), (cmd) =>
            {

            });

        }
        private void CreateTempTableForSupplierCodes(string tempTable)
        {
            StringBuilder queryBuilder = new StringBuilder(@"
                            IF NOT OBJECT_ID('#TempTable#', N'U') IS NOT NULL
	                            BEGIN
                                 CREATE TABLE #TempTable# ( 
                         
	                             [ID] [bigint] IDENTITY(1,1) NOT NULL,
	                             [Code] [varchar](20) NOT NULL,
	                             [ZoneID] [bigint] NOT NULL,
	                             [BED] [datetime] NOT NULL,
	                             [EED] [datetime] NULL,
                                 [IsUpdated] [Bit] Null)
                                END
                                ");
            queryBuilder.Replace("#TempTable#", tempTable);
            ExecuteNonQueryText(queryBuilder.ToString(), (cmd) =>
            {

            });

        }
        private void CreateTempTableForSupplierRates(string tempTable)
        {
            StringBuilder queryBuilder = new StringBuilder(@"
                            IF NOT OBJECT_ID('#TempTable#', N'U') IS NOT NULL
	                            BEGIN
                                     CREATE TABLE #TempTable# ( 
                        	        [ID] [bigint] IDENTITY(1,1) NOT NULL,
	                                [PriceListID] [int] NOT NULL,
	                                [ZoneID] [bigint] NOT NULL,
	                                [Rate] [decimal](9, 5) NOT NULL,
	                                [BED] [datetime] NOT NULL,
	                                [EED] [datetime] NULL,
                                    [IsUpdated] [Bit] Null)
                                END
                             
                                ");
            queryBuilder.Replace("#TempTable#", tempTable);
            ExecuteNonQueryText(queryBuilder.ToString(), (cmd) =>
            {

            });

        }

        private object InitialiazeZonesStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }
        private object InitialiazeCodesStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }
        private object InitialiazeRatesStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        private void WriteRecordToZonesStream(Zone record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}",
                       record.SupplierZoneId,
                       record.Name,
                       record.SupplierId,
                       record.BeginEffectiveDate,
                       record.EndEffectiveDate,
                       (record.Status != Status.New) ? 1 : 0);
        }
        private void WriteRecordToCodesStream(Code record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}",
                       0,
                       record.CodeValue,
                       record.ZoneId,
                       record.BeginEffectiveDate,
                       record.EndEffectiveDate,
                        (record.Status != Status.New) ? 1 : 0);
        }
        private void WriteRecordToRatesStream(Rate record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}",
                       0,
                       record.PriceListId,
                       record.ZoneId,
                       record.NormalRate,
                       record.BeginEffectiveDate,
                       record.EndEffectiveDate,
                       (record.Status != Status.New)?1:0);
        }

        private object FinishDBApplyStream(object dbApplyStream, string tempTable)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = tempTable,
                Stream = streamForBulkInsert,
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = '^',
            };
        }
        private void ApplySupplierZonesForTempDB(object preparedObject)
        {
            InsertBulkToTable(preparedObject as BaseBulkInsertInfo);
        }

        public string BuildApplyZonesQuery(string tempTable)
        {
            StringBuilder queryBuilder = new StringBuilder(@"
                                        INSERT INTO [TOneWhS_BE].[SupplierZone] 
                                        (ID,Name,SupplierID,BED,EED)
                                        Select ID,Name,SupplierID,BED,EED From #TableName# WHERE IsUpdated=0
                                        UPDATE sz
                                        SET sz.EED = tsz.EED
                                        FROM [TOneWhS_BE].[SupplierZone] sz INNER JOIN #TableName# tsz ON sz.ID=tsz.ID  
                                        Where tsz.IsUpdated=1

                                        DROP TABLE #TableName#
                                        
                                ");
            queryBuilder.Replace("#TableName#",tempTable);
            return queryBuilder.ToString();
        }
        public string BuildApplyCodesQuery(string tempTable)
        {
            StringBuilder queryBuilder = new StringBuilder(@"
                                        INSERT INTO [TOneWhS_BE].[SupplierCode] 
                                        (Code,ZoneID,BED,EED)
                                        Select Code,ZoneID,BED,EED From #TableName# WHERE IsUpdated=0
                                        UPDATE sc
                                        SET sc.EED = tsc.EED
                                        FROM [TOneWhS_BE].[SupplierCode] sc INNER JOIN #TableName# tsc ON sc.ID=tsc.ID 
                                        Where tsc.IsUpdated=1
                                        DROP TABLE #TableName#

                                        
                                ");
            queryBuilder.Replace("#TableName#", tempTable);
            return queryBuilder.ToString();
        }
        public string BuildApplyRatesQuery(string tempTable)
        {
            StringBuilder queryBuilder = new StringBuilder(@"
                                        INSERT INTO [TOneWhS_BE].[SupplierRate] 
                                        (PriceListID,ZoneID,Rate,BED,EED)
                                        Select PriceListID,ZoneID,Rate,BED,EED From #TableName# WHERE IsUpdated=0
                                        UPDATE sr
                                        SET sr.EED = tsr.EED
                                        FROM [TOneWhS_BE].[SupplierRate] sr INNER JOIN #TableName# tsr ON sr.ID=tsr.ID  
                                        Where tsr.IsUpdated=1
                                     DROP TABLE #TableName#
                                      
                                ");
            queryBuilder.Replace("#TableName#", tempTable);
            return queryBuilder.ToString();
        }

        public void MergingDataAndFinalizing()
        {
            ExecuteNonQueryText(BuildApplyZonesQuery(tempZoneTable), (cmd) =>
            {
            });
            ExecuteNonQueryText(BuildApplyCodesQuery(tempCodeTable), (cmd) =>
            {
            });
            ExecuteNonQueryText(BuildApplyRatesQuery(tempRateTable), (cmd) =>
            {
            });

        }

        const string tempZoneTable = "[dbo].[TempTableForSupplierZones_TOneWhS]";
        const string tempCodeTable = "[dbo].[TempTableForSupplierCodes_TOneWhS]";
        const string tempRateTable = "[dbo].[TempTableForSupplierRates_TOneWhS]";
    }
}

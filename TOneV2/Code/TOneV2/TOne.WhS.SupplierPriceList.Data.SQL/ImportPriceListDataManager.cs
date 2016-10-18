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

        public void InsertPriceListObject(List<Zone> supplierZones,List<Code> codesToBeDeleted,int supplierId,int priceListId)
        {
            GenerateTempTablesName(supplierId);

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
                     if (rate.Status == Status.New)
                        rate.PriceListId = priceListId;
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
                                 [CountryID] [int] NOT NULL,
	                             [Name] [nvarchar](255) NOT NULL,
	                             [SupplierID] [int] NOT NULL,
	                             [BED] [datetime] NOT NULL,
	                             [EED] [datetime] NULL,
                                 [IsUpdated] [Bit] Null)
                                ALTER TABLE #TempTable#
                                ADD PRIMARY KEY (ID)
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
                         
	                             [ID] [bigint] NOT NULL,
	                             [Code] [varchar](20) NOT NULL,
	                             [ZoneID] [bigint] NOT NULL,
                                 [CodeGroupID] [int] NULL,
	                             [BED] [datetime] NOT NULL,
	                             [EED] [datetime] NULL,
                                 [IsUpdated] [Bit] Null)
                               
                                 CREATE CLUSTERED INDEX IX_SupplierCode_ID ON #TempTable# (ID); 
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
                        	           [ID] [bigint]  NOT NULL,
	                                   [PriceListID] [int] NOT NULL,
	                                   [ZoneID] [bigint] NOT NULL,
                                       [CurrencyID] [int] NULL,
	                                   [NormalRate] [decimal](9, 5) NOT NULL,
	                                   [BED] [datetime] NOT NULL,
	                                   [EED] [datetime] NULL,
                                       [IsUpdated] [Bit] Null)
                                 
                                    CREATE CLUSTERED INDEX IX_SupplierRate_ID ON #TempTable# (ID); 

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
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}",
                       record.SupplierZoneId,
                       record.CountryId,
                       record.Name,
                       record.SupplierId,
                       record.BeginEffectiveDate,
                       record.EndEffectiveDate,
                       (record.Status != Status.New) ? 1 : 0);
        }
        private void WriteRecordToCodesStream(Code record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}",
                       record.SupplierCodeId,
                       record.CodeValue,
                       record.ZoneId,
                       record.CodeGroupId,
                       record.BeginEffectiveDate,
                       record.EndEffectiveDate,
                        (record.Status != Status.New) ? 1 : 0);
        }
        private void WriteRecordToRatesStream(Rate record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}",
                       record.SupplierRateId,
                       record.PriceListId,
                       record.ZoneId,
                       record.CurrencyID,
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
                                        (ID,CountryID,Name,SupplierID,BED,EED)
                                        Select ID,CountryID,Name,SupplierID,BED,EED From #TableName# WHERE IsUpdated=0
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
                                        (Code,ZoneID,CodeGroupID,BED,EED)
                                        Select Code,ZoneID,CodeGroupID,BED,EED From #TableName# WHERE IsUpdated=0
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
                                        (PriceListID,ZoneID,CurrencyID,NormalRate,BED,EED)
                                        Select PriceListID,ZoneID,CurrencyID,NormalRate,BED,EED From #TableName# WHERE IsUpdated=0
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
            ExecuteNonQueryText(BuildApplyZonesQuery(tempZoneTable),null);
            ExecuteNonQueryText(BuildApplyCodesQuery(tempCodeTable),null);
            ExecuteNonQueryText(BuildApplyRatesQuery(tempRateTable),null);

        }

        private void GenerateTempTablesName(int supplierID)
        {
            this.tempZoneTable= String.Format("[dbo].[TempTableForSupplierZones_{0}_{1}]", supplierID, Guid.NewGuid());
            this.tempCodeTable = String.Format("[dbo].[TempTableForSupplierCodes_{0}_{1}]", supplierID, Guid.NewGuid());
            this.tempRateTable = String.Format("[dbo].[TempTableForSupplierRates_{0}_{1}]", supplierID, Guid.NewGuid());
        }
        private string tempZoneTable;
        private string tempCodeTable;
        private string tempRateTable;
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.WhS.CodePreparation.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.CodePreparation.Data.SQL
{
    public class CodePreparationDataManager : BaseTOneDataManager, ICodePreparationDataManager
    {
        public CodePreparationDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        public object InitialiazeZonesStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }
        public object InitialiazeCodesStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToZonesStream(Zone record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}",
                       record.SaleZoneId,
                       record.SellingNumberPlanId,
                       record.CountryId,
                       record.Name,
                       record.BeginEffectiveDate,
                       record.EndEffectiveDate,
                       (record.Status != Status.New) ? 1 : 0);
        }

        public void WriteRecordToCodesStream(Code record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}",
                       record.SaleCodeId,
                       record.CodeValue,
                       record.ZoneId,
                       record.CodeGroupId,
                       record.BeginEffectiveDate,
                       record.EndEffectiveDate,
                       (record.Status != Status.New) ? 1 : 0);
        }

        public void ApplySaleZonesForDB(object preparedSaleZones)
        {
            InsertBulkToTable(preparedSaleZones as BaseBulkInsertInfo);
        }

        public void InsertCodePreparationObject(Dictionary<string, Zone> saleZones, int sellingNumberPlanId)
        {
            GenerateTempTablesName(sellingNumberPlanId);

            CreateTempTableForSaleZones(tempZoneTable);
            CreateTempTableForSaleCodes(tempCodeTable);

            object dbApplyStreamZones = InitialiazeZonesStreamForDBApply();
            object dbApplyStreamCodes = InitialiazeCodesStreamForDBApply();

            foreach (var zone in saleZones)
            {
                WriteRecordToZonesStream(zone.Value, dbApplyStreamZones);
                foreach (Code code in zone.Value.Codes)
                {
                    WriteRecordToCodesStream(code, dbApplyStreamCodes);
                }
            }

            object prepareToApplyZones = FinishDBApplyStream(dbApplyStreamZones, tempZoneTable);
            object prepareToApplyCodes = FinishDBApplyStream(dbApplyStreamCodes, tempCodeTable);

            ApplySaleZonesForDB(prepareToApplyZones);
            ApplySaleZonesForDB(prepareToApplyCodes);

            MergingDataAndFinalizing();

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



        public string BuildApplyZonesQuery(string tempTable)
        {
            StringBuilder queryBuilder = new StringBuilder(@"
                                        INSERT INTO [TOneWhS_BE].[SaleZone] 
                                        (ID,SellingNumberPlanID,CountryID,Name,BED,EED)
                                        Select ID,SellingNumberPlanID,CountryID,Name,BED,EED From #TableName# WHERE IsUpdated=0
                                        UPDATE sz
                                        SET sz.EED = tsz.EED
                                        FROM [TOneWhS_BE].[SaleZone] sz INNER JOIN #TableName# tsz ON sz.ID=tsz.ID  
                                        Where tsz.IsUpdated=1

                                        DROP TABLE #TableName#
                                        
                                ");
            queryBuilder.Replace("#TableName#", tempTable);
            return queryBuilder.ToString();
        }
        public string BuildApplyCodesQuery(string tempTable)
        {
            StringBuilder queryBuilder = new StringBuilder(@"
                                        INSERT INTO [TOneWhS_BE].[SaleCode] 
                                        (Code,ZoneID,CodeGroupID,BED,EED)
                                        Select Code,ZoneID,CodeGroupID,BED,EED From #TableName# WHERE IsUpdated=0
                                        UPDATE sc
                                        SET sc.EED = tsc.EED
                                        FROM [TOneWhS_BE].[SaleCode] sc INNER JOIN #TableName# tsc ON sc.ID=tsc.ID 
                                        Where tsc.IsUpdated=1
                                        DROP TABLE #TableName#

                                        
                                ");
            queryBuilder.Replace("#TableName#", tempTable);
            return queryBuilder.ToString();
        }

        public void MergingDataAndFinalizing()
        {
            ExecuteNonQueryText(BuildApplyZonesQuery(tempZoneTable), null);
            ExecuteNonQueryText(BuildApplyCodesQuery(tempCodeTable), null);

        }
        private void CreateTempTableForSaleZones(string tempTable)
        {
            StringBuilder queryBuilder = new StringBuilder(@"
                        IF NOT OBJECT_ID('#TempTable#', N'U') IS NOT NULL
	                        BEGIN
                                 CREATE TABLE #TempTable# (              
	                             [ID] [bigint] NOT NULL,
                                 [SellingNumberPlanID] [int] NOT NULL,
	                             [CountryID] [int] NOT NULL,
	                             [Name] [nvarchar] (255) NOT NULL,
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
        private void CreateTempTableForSaleCodes(string tempTable)
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

                                 CREATE CLUSTERED INDEX IX_SaleCode_ID ON #TempTable# (ID); 

                                END
                                ");
            queryBuilder.Replace("#TempTable#", tempTable);
            ExecuteNonQueryText(queryBuilder.ToString(), (cmd) =>
            {

            });

        }

        private void GenerateTempTablesName(int sellingNumberPlanId)
        {
            this.tempZoneTable = String.Format("[dbo].[TempTableForSaleZones_{0}_{1}]", sellingNumberPlanId, Guid.NewGuid());
            this.tempCodeTable = String.Format("[dbo].[TempTableForSaleCodes_{0}_{1}]", sellingNumberPlanId, Guid.NewGuid());
        }
        private string tempZoneTable;
        private string tempCodeTable;
    }
}

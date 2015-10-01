using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.SupplierPriceList.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.SupplierPriceList.Data.SQL
{
    public class SupplierCodeDataManager : BaseTOneDataManager, ISupplierCodeDataManager
    {
        public SupplierCodeDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        public void InsertSupplierCodes(List<Zone> supplierZones)
        {
            string tempTable = "[dbo].[TempTableForSupplierCodes_TOneWhS]";
            CreateTempTableForSupplierCodes(tempTable);

            object dbApplyStream = InitialiazeStreamForDBApply();
            foreach (Zone zone in supplierZones)
            {
                foreach (Code code in zone.Codes)
                {
                    WriteRecordToStream(code, dbApplyStream);
                }
            }


            object prepareToApplySupplierCodes = FinishDBApplyStream(dbApplyStream, tempTable);
            ApplySupplierCodesForTempDB(prepareToApplySupplierCodes);
        }
        private void CreateTempTableForSupplierCodes(string tempTable)
        {
            StringBuilder queryBuilder = new StringBuilder(@"
                           CREATE TABLE #TempTable# ( 
                         
	                        [ID] [bigint] IDENTITY(1,1) NOT NULL,
	                        [Code] [varchar](20) NOT NULL,
	                        [ZoneID] [bigint] NOT NULL,
	                        [CodeGroupID] [int] NULL,
	                        [BED] [datetime] NOT NULL,
	                        [EED] [datetime] NULL,
                            [Status] [Char](1) Null,
                            CONSTRAINT [PK_SupplierCode] PRIMARY KEY CLUSTERED 
                            (
	                                [ID] ASC
                            )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
                                    ) ON [PRIMARY]

                                ");
            queryBuilder.Replace("#TempTable#", tempTable);
            ExecuteNonQueryText(queryBuilder.ToString(), (cmd) =>
            {

            });

        }

        private object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        private void WriteRecordToStream(Code record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}",
                       0,
                       record.CodeValue,
                       record.ZoneId,
                       record.BeginEffectiveDate,
                       record.EndEffectiveDate,
                       record.Status);
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

        private void ApplySupplierCodesForTempDB(object preparedSupplierCodes)
        {
            InsertBulkToTable(preparedSupplierCodes as BaseBulkInsertInfo);
        }
    }
}

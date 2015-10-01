using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.SupplierPriceList.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.SupplierPriceList.Data.SQL
{
    public class SupplierRateDataManager : BaseTOneDataManager, ISupplierRateDataManager
    {
        public SupplierRateDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }
        public void InsertSupplierRates(List<Zone> supplierZones)
        {
            string tempTable = "[dbo].[TempTableForSupplierRates_TOneWhS]";
            CreateTempTableForSupplierRates(tempTable);

            object dbApplyStream = InitialiazeStreamForDBApply();
            foreach (Zone zone in supplierZones)
            {
                foreach (Rate rate in zone.Rates)
                {
                    WriteRecordToStream(rate, dbApplyStream);
                }
            }

            object prepareToApplySupplierRates = FinishDBApplyStream(dbApplyStream, tempTable);
            ApplySupplierRatesForTempDB(prepareToApplySupplierRates);
        }

        private void CreateTempTableForSupplierRates(string tempTable)
        {
            StringBuilder queryBuilder = new StringBuilder(@"
                           CREATE TABLE #TempTable# ( 
                        	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	                        [PriceListID] [int] NOT NULL,
	                        [ZoneID] [bigint] NOT NULL,
	                        [Rate] [decimal](9, 5) NOT NULL,
	                        [OffPeakRate] [decimal](9, 5) NULL,
	                        [WeekendRate] [decimal](9, 5) NULL,
	                        [BED] [datetime] NOT NULL,
	                        [EED] [datetime] NULL,
                            [Status] [Char](1) Null,
                             CONSTRAINT [PK_SupplierRate] PRIMARY KEY CLUSTERED 
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
        private void WriteRecordToStream(Rate record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}",
                       0,
                       record.PriceListId,
                       record.ZoneId,
                       record.NormalRate,
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
        private void ApplySupplierRatesForTempDB(object preparedSupplierRates)
        {
            InsertBulkToTable(preparedSupplierRates as BaseBulkInsertInfo);
        }
    }
}

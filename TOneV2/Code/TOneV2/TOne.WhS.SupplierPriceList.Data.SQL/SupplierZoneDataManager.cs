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
    public class SupplierZoneDataManager : BaseTOneDataManager, ISupplierZoneDataManager
    {
        public SupplierZoneDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        public void InsertSupplierZones(List<Zone> supplierZones)
        {
            string tempTable = "[dbo].[TempTableForSupplierZones_TOneWhS]";
            CreateTempTableForSupplierZones(tempTable);

            object dbApplyStream = InitialiazeStreamForDBApply();
            foreach (Zone supplierZone in supplierZones)
                WriteRecordToStream(supplierZone, dbApplyStream);
            object prepareToApplySupplierZones = FinishDBApplyStream(dbApplyStream, tempTable);
            ApplySupplierZonesForTempDB(prepareToApplySupplierZones);
        }
        private void CreateTempTableForSupplierZones(string tempTable)
        {
            StringBuilder queryBuilder = new StringBuilder(@"
                           CREATE TABLE #TempTable# (              
	                        [ID] [bigint] IDENTITY(1,1) NOT NULL,
	                        [Name] [nvarchar](255) NOT NULL,
	                        [SupplierID] [int] NOT NULL,
	                        [BED] [datetime] NOT NULL,
	                        [EED] [datetime] NULL,
                            [Status] [Char](1) Null,
                            CONSTRAINT [PK_SupplierZone] PRIMARY KEY CLUSTERED 
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
        private void WriteRecordToStream(Zone record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}",
                       record.SupplierZoneId,
                       record.Name,
                       record.SupplierId,
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
        public void ApplySupplierZonesForTempDB(object preparedSupplierZones)
        {
            InsertBulkToTable(preparedSupplierZones as BaseBulkInsertInfo);
        }
    }
}

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
    public class SupplierZoneDataManager : BaseTOneDataManager, ISupplierZoneDataManager
    {
        public SupplierZoneDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        public void InsertSupplierZones(List<Zone> supplierZones)
        {
            object dbApplyStream = InitialiazeStreamForDBApply();
            foreach (Zone supplierZone in supplierZones)
                WriteRecordToStream(supplierZone, dbApplyStream);
            object prepareToApplySupplierZones = FinishDBApplyStream(dbApplyStream);
            ApplySupplierZonesForDB(prepareToApplySupplierZones);
        }

        public bool UpdateSupplierZones(int supplierId, DateTime effectiveDate)
        {
            int recordesEffected = ExecuteNonQuerySP("TOneWhS_BE.sp_SupplierZone_Update", supplierId, effectiveDate);
            return (recordesEffected > 0);
        }
        private object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }
        private void WriteRecordToStream(Zone record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}",
                       0,
                       record.Name,
                       record.SupplierId,
                       record.BeginEffectiveDate,
                       record.EndEffectiveDate);
        }
        private object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "[TOneWhS_BE].[SupplierZone]",
                Stream = streamForBulkInsert,
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = '^',
            };
        }
        public void ApplySupplierZonesForDB(object preparedSupplierZones)
        {
            InsertBulkToTable(preparedSupplierZones as BaseBulkInsertInfo);
        }
    }
}

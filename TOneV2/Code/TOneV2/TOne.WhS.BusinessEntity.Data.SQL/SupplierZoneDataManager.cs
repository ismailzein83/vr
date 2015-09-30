using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class SupplierZoneDataManager:BaseSQLDataManager,ISupplierZoneDataManager
    {
        public SupplierZoneDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {
          
        }

        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(SupplierZone record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}",
                       0,
                       record.Name,
                       record.SupplierId,
                       record.BeginEffectiveDate,
                       record.EndEffectiveDate);
        }
        public object FinishDBApplyStream(object dbApplyStream)
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
        public void InsertSupplierZones(List<SupplierZone> supplierZones)
        {
            object dbApplyStream = InitialiazeStreamForDBApply();
            foreach (SupplierZone supplierZone in supplierZones)
                WriteRecordToStream(supplierZone, dbApplyStream);
            object prepareToApplySupplierZones = FinishDBApplyStream(dbApplyStream);
            ApplySupplierZonesForDB(prepareToApplySupplierZones);
        }

        public List<SupplierZone> GetSupplierZones(int supplierId, DateTime effectiveDate)
        {
            return GetItemsSP("TOneWhS_BE.sp_SupplierZone_GetBySupplierId", SupplierZoneMapper, supplierId, effectiveDate);
        }
        SupplierZone SupplierZoneMapper(IDataReader reader)
        {
            SupplierZone supplierZone = new SupplierZone
            {
                SupplierId = (int)reader["SupplierID"],
                SupplierZoneId = (long)reader["ID"],
                Name = reader["Name"] as string,
                BeginEffectiveDate = GetReaderValue<DateTime>(reader, "BED"),
                EndEffectiveDate = GetReaderValue<DateTime?>(reader, "EED")
            };
            return supplierZone;
        }


        public bool UpdateSupplierZones(int supplierId, DateTime effectiveDate)
        {
            int recordesEffected = ExecuteNonQuerySP("TOneWhS_BE.sp_SupplierZone_Update", supplierId, effectiveDate);
            return (recordesEffected > 0);
        }

        public int ReserveIDRange(int numberOfIDs)
        {
            return (int)ExecuteScalarSP("TOneWhS_BE.sp_SupplierZoneIDManager_ReserveIDRange", numberOfIDs);
        }
    }
}

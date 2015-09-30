using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class SupplierCodeDataManager : BaseTOneDataManager, ISupplierCodeDataManager
    {
        public SupplierCodeDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {
          
        }
        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(SupplierCode record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}",
                       0,
                       record.Code,
                       record.ZoneId,
                       record.BeginEffectiveDate,
                       record.EndEffectiveDate);
        }
        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "[TOneWhS_BE].[SupplierCode]",
                Stream = streamForBulkInsert,
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = '^',
            };
        }

        public void ApplySupplierCodesForDB(object preparedSupplierCodes)
        {
            InsertBulkToTable(preparedSupplierCodes as BaseBulkInsertInfo);
        }
        public void InsertSupplierCodes(List<SupplierCode> supplierCodes)
        {
            object dbApplyStream = InitialiazeStreamForDBApply();
            foreach (SupplierCode supplierCode in supplierCodes)
                WriteRecordToStream(supplierCode, dbApplyStream);
            object prepareToApplySupplierCodes = FinishDBApplyStream(dbApplyStream);
            ApplySupplierCodesForDB(prepareToApplySupplierCodes);
        }
        public bool UpdateSupplierCodes(List<long> supplierZoneIds, DateTime effectiveDate)
        {
            string zoneIds = null;
            if (supplierZoneIds != null && supplierZoneIds.Count > 0)
                zoneIds = string.Join<long>(",", supplierZoneIds);
            int recordesEffected = ExecuteNonQuerySP("TOneWhS_BE.sp_SupplierCode_Update", zoneIds, effectiveDate);
            return (recordesEffected > 0);
        }


        public List<SupplierCode> GetSupplierCodes(DateTime minimumDate)
        {
            return GetItemsSP("TOneWhS_BE.sp_SupplierCode_GetByDate", SupplierCodeMapper, minimumDate);
        }
        SupplierCode SupplierCodeMapper(IDataReader reader)
        {
            SupplierCode supplierCode = new SupplierCode
            {
                Code = GetReaderValue<string>(reader, "Code"),
                SupplierCodeId = GetReaderValue<int>(reader, "ID"),
                ZoneId = (long)reader["ZoneID"],
                BeginEffectiveDate = GetReaderValue<DateTime>(reader, "BED"),
                EndEffectiveDate = GetReaderValue<DateTime?>(reader, "EED"),
                
            };
            return supplierCode;
        }
    }
}

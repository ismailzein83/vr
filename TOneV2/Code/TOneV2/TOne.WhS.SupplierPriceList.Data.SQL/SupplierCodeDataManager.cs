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
    public class SupplierCodeDataManager : BaseTOneDataManager, ISupplierCodeDataManager
    {
        public SupplierCodeDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }
     
        public void InsertSupplierCodes(List<Code> supplierCodes)
        {
            object dbApplyStream = InitialiazeStreamForDBApply();
            foreach (Code supplierCode in supplierCodes)
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
        private object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        private void WriteRecordToStream(Code record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}",
                       0,
                       record.CodeValue,
                       record.ZoneId,
                       record.BeginEffectiveDate,
                       record.EndEffectiveDate);
        }
        private object FinishDBApplyStream(object dbApplyStream)
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

        private void ApplySupplierCodesForDB(object preparedSupplierCodes)
        {
            InsertBulkToTable(preparedSupplierCodes as BaseBulkInsertInfo);
        }
    }
}

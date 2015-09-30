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
    public class SupplierRateDataManager : BaseSQLDataManager, ISupplierRateDataManager
    {
        public SupplierRateDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {
          
        }
        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(SupplierRate record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}",
                       0,
                       record.PriceListId,
                       record.ZoneId,
                       record.NormalRate,
                       record.BeginEffectiveDate,
                       record.EndEffectiveDate);
        }
        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "[TOneWhS_BE].[SupplierRate]",
                Stream = streamForBulkInsert,
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = '^',
            };
        }

        public void ApplySupplierRatesForDB(object preparedSupplierRates)
        {
            InsertBulkToTable(preparedSupplierRates as BaseBulkInsertInfo);
        }
        public void InsertSupplierRates(List<SupplierRate> supplierRates)
        {
            object dbApplyStream = InitialiazeStreamForDBApply();
            foreach (SupplierRate supplierRate in supplierRates)
                WriteRecordToStream(supplierRate, dbApplyStream);
            object prepareToApplySupplierRates = FinishDBApplyStream(dbApplyStream);
            ApplySupplierRatesForDB(prepareToApplySupplierRates);
        }


        public bool UpdateSupplierRates(List<long> zoneIdsList, DateTime effectiveDate)
        {
            string zoneIds = null;
            if (zoneIdsList != null && zoneIdsList.Count > 0)
                zoneIds = string.Join<long>(",", zoneIdsList);
            int recordesEffected = ExecuteNonQuerySP("TOneWhS_BE.sp_SupplierRate_Update", zoneIds, effectiveDate);
            return (recordesEffected > 0);
        }
        public List<SupplierRate> GetSupplierRates(DateTime minimumDate)
        {
            return GetItemsSP("TOneWhS_BE.sp_SupplierRate_GetByDate", SupplierRateMapper, minimumDate);
        }
        SupplierRate SupplierRateMapper(IDataReader reader)
        {
            SupplierRate supplierRate = new SupplierRate
            {
                NormalRate = GetReaderValue<decimal>(reader, "Rate"),
                SupplierRateId = (long)reader["ID"],
                ZoneId = (long)reader["ZoneID"],
                PriceListId = GetReaderValue<int>(reader, "BED"),
                BeginEffectiveDate = GetReaderValue<DateTime>(reader, "BED"),
                EndEffectiveDate = GetReaderValue<DateTime?>(reader, "EED")
            };
            return supplierRate;
        }
    }
}

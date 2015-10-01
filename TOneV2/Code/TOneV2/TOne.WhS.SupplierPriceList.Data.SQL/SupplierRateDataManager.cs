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
    public class SupplierRateDataManager : BaseTOneDataManager, ISupplierRateDataManager
    {
        public SupplierRateDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }
        public void InsertSupplierRates(List<Rate> supplierRates)
        {
            object dbApplyStream = InitialiazeStreamForDBApply();
            foreach (Rate supplierRate in supplierRates)
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
        private object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }
        private void WriteRecordToStream(Rate record, object dbApplyStream)
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
        private object FinishDBApplyStream(object dbApplyStream)
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
        private void ApplySupplierRatesForDB(object preparedSupplierRates)
        {
            InsertBulkToTable(preparedSupplierRates as BaseBulkInsertInfo);
        }
    }
}

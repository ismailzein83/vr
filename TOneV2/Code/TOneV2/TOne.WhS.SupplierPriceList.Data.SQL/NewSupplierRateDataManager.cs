using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.Data.SQL;

namespace TOne.WhS.SupplierPriceList.Data.SQL
{
    public class NewSupplierRateDataManager : BaseSQLDataManager, INewSupplierRateDataManager
    {
        readonly string[] _columns = { "ID", "ProcessInstanceID", "ZoneID", "CurrencyID", "NormalRate", "RateTypeID", "Change", "BED", "EED" ,"IsExcluded"};
        public NewSupplierRateDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        public long ProcessInstanceId
        {
            set
            {
                _processInstanceID = value;
            }
        }

        long _processInstanceID;

        public void ApplyNewRatesToDB(object preparedRates)
        {
            InsertBulkToTable(preparedRates as BaseBulkInsertInfo);
        }

        object Vanrise.Data.IBulkApplyDataManager<NewRate>.FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                ColumnNames = _columns,
                TableName = "TOneWhS_BE.SPL_SupplierRate_New",
                Stream = streamForBulkInsert,
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = '^',
            };
        }

        object Vanrise.Data.IBulkApplyDataManager<NewRate>.InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(NewRate record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^{8}^{9}",
                       record.RateId,
                       _processInstanceID,
                       record.Zone.ZoneId,
                       record.CurrencyId,
                       decimal.Round(record.NormalRate, 8),
                       record.RateTypeId,
                       (int)record.Change,
                       GetDateTimeForBCP(record.BED),
                       GetDateTimeForBCP(record.EED),
                       (record.IsExcluded) ? 1 : 0);
        }
    }
}

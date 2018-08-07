using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.Data.SQL;

namespace TOne.WhS.SupplierPriceList.Data.SQL
{
    public class ChangedSupplierRateDataManager : BaseSQLDataManager, IChangedSupplierRateDataManager
    {
        public ChangedSupplierRateDataManager()
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

        public void ApplyChangedRatesToDB(object preparedRates)
        {
            InsertBulkToTable(preparedRates as BaseBulkInsertInfo);
        }

        object Vanrise.Data.IBulkApplyDataManager<ChangedRate>.FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "TOneWhS_BE.SPL_SupplierRate_Changed",
                Stream = streamForBulkInsert,
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = '^',
            };
        }

        object Vanrise.Data.IBulkApplyDataManager<ChangedRate>.InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(ChangedRate record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}",
                       record.EntityId,
                       _processInstanceID,
                       GetDateTimeForBCP(record.EED),
                        (record.IsExcluded) ? 1 : 0);
        }
    }
}

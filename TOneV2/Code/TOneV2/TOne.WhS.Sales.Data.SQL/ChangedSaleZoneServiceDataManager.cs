using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;
using Vanrise.Data;
using Vanrise.Data.SQL;

namespace TOne.WhS.Sales.Data.SQL
{
    public class ChangedSaleZoneServiceDataManager : BaseSQLDataManager, IChangedSaleZoneServiceDataManager
    {
        #region Fields / Properties

        readonly string[] columns = { "ID", "ProcessInstanceID", "EED" };

        private long _processInstanceId;

        public long ProcessInstanceId
        {
            set { _processInstanceId = value; }
        }

        #endregion

        #region Constructors

        public ChangedSaleZoneServiceDataManager() :
            base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        #endregion

        #region Public Methods

        public void ApplyChangedSaleZoneServicesToDB(IEnumerable<ChangedSaleZoneService> changedSaleZoneServices)
        {
            IBulkApplyDataManager<ChangedSaleZoneService> bulkApplyDataManager = this;
            object streamForDBApply = bulkApplyDataManager.InitialiazeStreamForDBApply();

            foreach (var changedSaleZoneService in changedSaleZoneServices)
                bulkApplyDataManager.WriteRecordToStream(changedSaleZoneService, streamForDBApply);

            object bulkInsertInfo = bulkApplyDataManager.FinishDBApplyStream(streamForDBApply);
            this.InsertBulkToTable(bulkInsertInfo as StreamBulkInsertInfo);
        }

        #endregion

        #region Private Methods

        object Vanrise.Data.IBulkApplyDataManager<ChangedSaleZoneService>.InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        void Vanrise.Data.IBulkApplyDataManager<ChangedSaleZoneService>.WriteRecordToStream(ChangedSaleZoneService record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord
            (
                "{0}^{1}^{2}",
                record.SaleEntityServiceId,
                _processInstanceId,
                record.EED
            );
        }

        object Vanrise.Data.IBulkApplyDataManager<ChangedSaleZoneService>.FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "TOneWhS_Sales.RP_SaleZoneService_Changed",
                Stream = streamForBulkInsert,
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = '^',
                ColumnNames = columns
            };
        }

        #endregion
    }
}

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
    public class ChangedSaleZoneRoutingProductDataManager : BaseSQLDataManager, IChangedSaleZoneRoutingProductDataManager
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

        public ChangedSaleZoneRoutingProductDataManager() :
            base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        #endregion

        #region Public Methods

        public void ApplyChangedSaleZoneRoutingProductsToDB(IEnumerable<ChangedSaleZoneRoutingProduct> changedSaleZoneRoutingProducts)
        {
            IBulkApplyDataManager<ChangedSaleZoneRoutingProduct> bulkApplyDataManager = this;
            object streamForDBApply = bulkApplyDataManager.InitialiazeStreamForDBApply();

            foreach (var changedSaleZoneRoutingProduct in changedSaleZoneRoutingProducts)
                bulkApplyDataManager.WriteRecordToStream(changedSaleZoneRoutingProduct, streamForDBApply);

            object bulkInsertInfo = bulkApplyDataManager.FinishDBApplyStream(streamForDBApply);
            this.InsertBulkToTable(bulkInsertInfo as StreamBulkInsertInfo);
        }

        #endregion

        #region Private Methods

        object Vanrise.Data.IBulkApplyDataManager<ChangedSaleZoneRoutingProduct>.InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        void Vanrise.Data.IBulkApplyDataManager<ChangedSaleZoneRoutingProduct>.WriteRecordToStream(ChangedSaleZoneRoutingProduct record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord
            (
                "{0}^{1}^{2}",
                record.SaleEntityRoutingProductId,
                _processInstanceId,
				GetDateTimeForBCP(record.EED)
            );
        }

        object Vanrise.Data.IBulkApplyDataManager<ChangedSaleZoneRoutingProduct>.FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "TOneWhS_Sales.RP_SaleZoneRoutingProduct_Changed",
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

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
    public class NewSaleZoneRoutingProductDataManager : BaseSQLDataManager, INewSaleZoneRoutingProductDataManager
    {
        #region Fields / Properties

        readonly string[] columns = { "ID", "OwnerType", "OwnerID", "ProcessInstanceID", "RoutingProductID", "ZoneID", "BED", "EED" };

        private long _processInstanceId;

        public long ProcessInstanceId
        {
            set { _processInstanceId = value; }
        }

        #endregion

        #region Constructors

        public NewSaleZoneRoutingProductDataManager() :
            base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        #endregion

        #region Public Methods

        public void ApplyNewSaleZoneRoutingProductsToDB(IEnumerable<NewSaleZoneRoutingProduct> newSaleZoneRoutingProducts)
        {
            IBulkApplyDataManager<NewSaleZoneRoutingProduct> bulkApplyDataManager = this;
            object streamForDBApply = bulkApplyDataManager.InitialiazeStreamForDBApply();

            foreach (var newSaleZoneRoutingProduct in newSaleZoneRoutingProducts)
                bulkApplyDataManager.WriteRecordToStream(newSaleZoneRoutingProduct, streamForDBApply);

            object bulkInsertInfo = bulkApplyDataManager.FinishDBApplyStream(streamForDBApply);
            this.InsertBulkToTable(bulkInsertInfo as StreamBulkInsertInfo);
        }

        #endregion

        #region Private Methods

        object Vanrise.Data.IBulkApplyDataManager<NewSaleZoneRoutingProduct>.InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        void Vanrise.Data.IBulkApplyDataManager<NewSaleZoneRoutingProduct>.WriteRecordToStream(NewSaleZoneRoutingProduct record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord
            (
                "{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}",
                record.SaleEntityRoutingProductId,
                (int)record.OwnerType,
                record.OwnerId,
                _processInstanceId,
                record.RoutingProductId,
                record.SaleZoneId,
				GetDateTimeForBCP(record.BED),
				GetDateTimeForBCP(record.EED)
            );
        }

        object Vanrise.Data.IBulkApplyDataManager<NewSaleZoneRoutingProduct>.FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "TOneWhS_Sales.RP_SaleZoneRoutingProduct_New",
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

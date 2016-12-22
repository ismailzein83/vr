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
    public class NewDefaultRoutingProductDataManager : BaseSQLDataManager, INewDefaultRoutingProductDataManager
    {
        #region Fields / Properties

        readonly string[] columns = { "ID", "ProcessInstanceID", "RoutingProductID", "BED", "EED" };

        private long _processInstanceId;

        public long ProcessInstanceId
        {
            set { _processInstanceId = value; }
        }

        #endregion

        #region Constructors

        public NewDefaultRoutingProductDataManager() :
            base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        #endregion

        #region Public Methods

        public void ApplyNewDefaultRoutingProductsToDB(IEnumerable<Entities.NewDefaultRoutingProduct> newDefaultRoutingProducts)
        {
            IBulkApplyDataManager<NewDefaultRoutingProduct> bulkApplyDataManager = this;
            object streamForDBApply = bulkApplyDataManager.InitialiazeStreamForDBApply();

            foreach (var newDefaultRoutingProduct in newDefaultRoutingProducts)
                bulkApplyDataManager.WriteRecordToStream(newDefaultRoutingProduct, streamForDBApply);

            object bulkInsertInfo = bulkApplyDataManager.FinishDBApplyStream(streamForDBApply);
            this.InsertBulkToTable(bulkInsertInfo as StreamBulkInsertInfo);
        }

        public bool Insert(NewDefaultRoutingProduct newDefaultRoutingProduct)
        {
            int affectedRows = ExecuteNonQuerySP("TOneWhS_Sales.sp_NewDefaultRoutingProduct_Insert", newDefaultRoutingProduct.SaleEntityRoutingProductId, _processInstanceId, newDefaultRoutingProduct.RoutingProductId, newDefaultRoutingProduct.BED, newDefaultRoutingProduct.EED);

            return affectedRows > 0;
        }

        #endregion

        #region Private Methods

        object Vanrise.Data.IBulkApplyDataManager<NewDefaultRoutingProduct>.InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        void Vanrise.Data.IBulkApplyDataManager<NewDefaultRoutingProduct>.WriteRecordToStream(Entities.NewDefaultRoutingProduct record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord
            (
                "{0}^{1}^{2}^{3}^{4}",
                record.SaleEntityRoutingProductId,
                _processInstanceId,
                record.RoutingProductId,
				GetDateTimeForBCP(record.BED),
				GetDateTimeForBCP(record.EED)
            );
        }

        object Vanrise.Data.IBulkApplyDataManager<NewDefaultRoutingProduct>.FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "TOneWhS_Sales.RP_DefaultRoutingProduct_New",
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

﻿using System;
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
                record.BED,
                record.EED
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
            };
        }

        #endregion
    }
}

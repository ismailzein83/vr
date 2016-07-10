﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;
using Vanrise.Data;
using Vanrise.Data.SQL;

namespace TOne.WhS.Sales.Data.SQL
{
    public class SaleZoneRoutingProductPreviewDataManager : BaseSQLDataManager, ISaleZoneRoutingProductPreviewDataManager
    {
        #region Fields / Properties

        private long _processInstanceId;

        public long ProcessInstanceId
        {
            set
            {
                _processInstanceId = value;
            }
        }

        #endregion

        #region Constructors

        public SaleZoneRoutingProductPreviewDataManager() :
            base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        #endregion

        #region Public Methods

        public IEnumerable<SaleZoneRoutingProductPreview> GetSaleZoneRoutingProductPreviews(RatePlanPreviewQuery query)
        {
            return GetItemsSP("TOneWhS_Sales.sp_SaleZoneRoutingProduct_GetPreviews", SaleZoneRoutingProductPreviewMapper, query.ProcessInstanceId);
        }

        public void ApplySaleZoneRoutingProductPreviewsToDB(IEnumerable<SaleZoneRoutingProductPreview> saleZoneRoutingProductPreviews)
        {
            IBulkApplyDataManager<SaleZoneRoutingProductPreview> bulkApplyDataManager = this;
            object streamForDBApply = bulkApplyDataManager.InitialiazeStreamForDBApply();

            foreach (var saleZoneRoutingProductPreview in saleZoneRoutingProductPreviews)
                bulkApplyDataManager.WriteRecordToStream(saleZoneRoutingProductPreview, streamForDBApply);

            object bulkInsertInfo = bulkApplyDataManager.FinishDBApplyStream(streamForDBApply);
            this.InsertBulkToTable(bulkInsertInfo as StreamBulkInsertInfo);
        }

        #endregion

        #region Private Methods

        object Vanrise.Data.IBulkApplyDataManager<SaleZoneRoutingProductPreview>.InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        void Vanrise.Data.IBulkApplyDataManager<SaleZoneRoutingProductPreview>.WriteRecordToStream(SaleZoneRoutingProductPreview record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;

            int? isCurrentSaleZoneRoutingProductInherited = null;
            if (record.IsCurrentSaleZoneRoutingProductInherited.HasValue)
                isCurrentSaleZoneRoutingProductInherited = (record.IsCurrentSaleZoneRoutingProductInherited.Value) ? 1 : 0;

            streamForBulkInsert.WriteRecord
            (
                "{0}^{1}^{2}^{3}^{4}^{5}",
                record.ZoneName,
                _processInstanceId,
                record.CurrentSaleZoneRoutingProductName,
                isCurrentSaleZoneRoutingProductInherited,
                record.NewSaleZoneRoutingProductName,
                record.EffectiveOn
            );
        }

        object Vanrise.Data.IBulkApplyDataManager<SaleZoneRoutingProductPreview>.FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "TOneWhS_Sales.RP_SaleZoneRoutingProduct_Preview",
                Stream = streamForBulkInsert,
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = '^',
            };
        }

        #endregion

        #region Mappers

        private SaleZoneRoutingProductPreview SaleZoneRoutingProductPreviewMapper(IDataReader reader)
        {
            return new SaleZoneRoutingProductPreview()
            {
                ZoneName = reader["ZoneName"] as string,
                CurrentSaleZoneRoutingProductName = reader["CurrentSaleZoneRoutingProductName"] as string,
                IsCurrentSaleZoneRoutingProductInherited = GetReaderValue<bool?>(reader, "IsCurrentSaleZoneRoutingProductInherited"),
                NewSaleZoneRoutingProductName = reader["NewSaleZoneRoutingProductName"] as string,
                EffectiveOn = (DateTime)reader["EffectiveOn"]
            };
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.Data;
using Vanrise.Data.SQL;

namespace TOne.WhS.Sales.Data.SQL
{
    public class SaleZoneServicePreviewDataManager : BaseSQLDataManager, ISaleZoneServicePreviewDataManager
    {
        #region Fields / Properties

        readonly string[] columns = { "ZoneName", "ProcessInstanceID", "CurrentServices", "IsCurrentServiceInherited", "NewServices", "EffectiveOn", "EffectiveUntil" };

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

        public SaleZoneServicePreviewDataManager() :
            base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        #endregion

        #region Public Methods

        public IEnumerable<SaleZoneServicePreview> GetSaleZoneServicePreviews(RatePlanPreviewQuery query)
        {
            return GetItemsSP("TOneWhS_Sales.sp_SaleZoneService_GetPreviews", SaleZoneServicePreviewMapper, query.ProcessInstanceId);
        }

        public void ApplySaleZoneServicePreviewsToDB(IEnumerable<SaleZoneServicePreview> saleZoneServicePreviews)
        {
            IBulkApplyDataManager<SaleZoneServicePreview> bulkApplyDataManager = this;
            object streamForDBApply = bulkApplyDataManager.InitialiazeStreamForDBApply();

            foreach (var saleZoneServicePreview in saleZoneServicePreviews)
                bulkApplyDataManager.WriteRecordToStream(saleZoneServicePreview, streamForDBApply);

            object bulkInsertInfo = bulkApplyDataManager.FinishDBApplyStream(streamForDBApply);
            this.InsertBulkToTable(bulkInsertInfo as StreamBulkInsertInfo);
        }

        #endregion

        #region Private Methods

        object Vanrise.Data.IBulkApplyDataManager<SaleZoneServicePreview>.InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        void Vanrise.Data.IBulkApplyDataManager<SaleZoneServicePreview>.WriteRecordToStream(SaleZoneServicePreview record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;

            string isCurrentServiceInherited = null;
            if (record.IsCurrentServiceInherited.HasValue)
                isCurrentServiceInherited = (record.IsCurrentServiceInherited.Value) ? "1" : "0";

            streamForBulkInsert.WriteRecord
            (
                "{0}^{1}^{2}^{3}^{4}^{5}^{6}",
                record.ZoneName,
                _processInstanceId,
                record.CurrentServices != null ? Vanrise.Common.Serializer.Serialize(record.CurrentServices) : null,
                isCurrentServiceInherited,
                record.NewServices != null ? Vanrise.Common.Serializer.Serialize(record.NewServices) : null,
                record.EffectiveOn,
                record.EffectiveUntil
            );
        }

        object Vanrise.Data.IBulkApplyDataManager<SaleZoneServicePreview>.FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "TOneWhS_Sales.RP_SaleZoneService_Preview",
                Stream = streamForBulkInsert,
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = '^',
                ColumnNames = columns
            };
        }

        #endregion

        #region Mappers

        private SaleZoneServicePreview SaleZoneServicePreviewMapper(IDataReader reader)
        {
            var preview = new SaleZoneServicePreview()
            {
                ZoneName = reader["ZoneName"] as string,
                IsCurrentServiceInherited = GetReaderValue<bool?>(reader, "IsCurrentServiceInherited"),
                EffectiveOn = (DateTime)reader["EffectiveOn"],
                EffectiveUntil = GetReaderValue<DateTime?>(reader, "EffectiveUntil")
            };

            string currentServices = reader["CurrentServices"] as string;
            if (currentServices != null)
                preview.CurrentServices = Vanrise.Common.Serializer.Deserialize<List<ZoneService>>(currentServices);

            string newServices = reader["NewServices"] as string;
            if (newServices != null)
                preview.NewServices = Vanrise.Common.Serializer.Deserialize<List<ZoneService>>(newServices);

            return preview;
        }

        #endregion
    }
}

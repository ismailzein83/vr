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
    public class RatePreviewDataManager : BaseSQLDataManager, IRatePreviewDataManager
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

        public RatePreviewDataManager() :
            base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        #endregion

        #region Public Methods

        public IEnumerable<RatePreview> GetRatePreviews(RatePlanPreviewQuery query)
        {
            return GetItemsSP("TOneWhS_Sales.sp_SaleRate_GetPreviews", RatePreviewMapper, query.ProcessInstanceId);
        }

        public void ApplyRatePreviewsToDB(IEnumerable<RatePreview> ratePreviews)
        {
            IBulkApplyDataManager<RatePreview> bulkApplyDataManager = this;
            object streamForDBApply = bulkApplyDataManager.InitialiazeStreamForDBApply();

            foreach (var ratePreview in ratePreviews)
                bulkApplyDataManager.WriteRecordToStream(ratePreview, streamForDBApply);

            object bulkInsertInfo = bulkApplyDataManager.FinishDBApplyStream(streamForDBApply);
            this.InsertBulkToTable(bulkInsertInfo as StreamBulkInsertInfo);
        }

        #endregion

        #region Private Methods

        object Vanrise.Data.IBulkApplyDataManager<RatePreview>.InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        void Vanrise.Data.IBulkApplyDataManager<RatePreview>.WriteRecordToStream(RatePreview record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;

            int? changeType = null;
            if (record.ChangeType.HasValue)
                changeType = Convert.ToInt32(record.ChangeType.Value);
            
            int? isCurrentRateInherited = null;
            if (record.IsCurrentRateInherited.HasValue)
                isCurrentRateInherited = (record.IsCurrentRateInherited.Value) ? 1 : 0;

            streamForBulkInsert.WriteRecord
            (
                "{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}",
                record.ZoneName,
                _processInstanceId,
                record.CurrentRate,
                isCurrentRateInherited,
                record.NewRate,
                changeType,
                record.EffectiveOn,
                record.EffectiveUntil
            );
        }

        object Vanrise.Data.IBulkApplyDataManager<RatePreview>.FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "TOneWhS_Sales.RP_SaleRate_Preview",
                Stream = streamForBulkInsert,
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = '^',
            };
        }

        #endregion

        #region Mappers

        private RatePreview RatePreviewMapper(IDataReader reader)
        {
            return new RatePreview()
            {
                ZoneName = reader["ZoneName"] as string,
                CurrentRate = GetReaderValue<decimal?>(reader, "CurrentRate"),
                IsCurrentRateInherited = GetReaderValue<bool?>(reader, "IsCurrentRateInherited"),
                NewRate = GetReaderValue<decimal?>(reader, "NewRate"),
                ChangeType = GetRateChangeType(reader["ChangeType"]), // Is this a good practice?
                EffectiveOn = (DateTime)reader["EffectiveOn"],
                EffectiveUntil = GetReaderValue<DateTime?>(reader, "EffectiveUntil")
            };
        }

        private RateChangeType? GetRateChangeType(object changeTypeAsObject)
        {
            RateChangeType? changeType = null;
            if (changeTypeAsObject != DBNull.Value)
            {
                int changeTypeAsInt = Convert.ToInt32(changeTypeAsObject);
                changeType = (RateChangeType)changeTypeAsInt;
            }
            return changeType;
        }

        #endregion
    }
}

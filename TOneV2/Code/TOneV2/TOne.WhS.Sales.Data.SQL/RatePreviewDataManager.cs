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

        readonly string[] columns = { "ZoneName", "ProcessInstanceID", "RateTypeID", "CurrentRate", "IsCurrentRateInherited", "NewRate", "ChangeType", "EffectiveOn", "EffectiveUntil" };

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

        public IEnumerable<RatePreview> GetRatePreviews(RatePreviewQuery query)
        {
            return GetItemsSP("TOneWhS_Sales.sp_SaleRate_GetPreviews", RatePreviewMapper, query.ProcessInstanceId, query.ZoneName);
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

            string isCurrentRateInherited = null;
            if (record.IsCurrentRateInherited.HasValue)
                isCurrentRateInherited = (record.IsCurrentRateInherited.Value) ? "1" : "0";

            streamForBulkInsert.WriteRecord
            (
                "{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^{8}",
                record.ZoneName,
                _processInstanceId,
                record.RateTypeId,
                GetRoundedRate(record.CurrentRate),
                isCurrentRateInherited,
                GetRoundedRate(record.NewRate),
                Convert.ToInt32(record.ChangeType),
				GetDateTimeForBCP(record.EffectiveOn),
				GetDateTimeForBCP(record.EffectiveUntil)
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
                ColumnNames = columns
            };
        }

        private decimal? GetRoundedRate(decimal? rate)
        {
            if (rate.HasValue)
                return decimal.Round(rate.Value, 8);
            return null;
        }

        #endregion

        #region Mappers

        private RatePreview RatePreviewMapper(IDataReader reader)
        {
            return new RatePreview()
            {
                ZoneName = reader["ZoneName"] as string,
                RateTypeId = GetReaderValue<int?>(reader, "RateTypeID"),
                CurrentRate = GetReaderValue<decimal?>(reader, "CurrentRate"),
                IsCurrentRateInherited = GetReaderValue<bool?>(reader, "IsCurrentRateInherited"),
                NewRate = GetReaderValue<decimal?>(reader, "NewRate"),
                ChangeType = (RateChangeType)reader["ChangeType"],
                EffectiveOn = GetReaderValue<DateTime?>(reader, "EffectiveOn"),
                EffectiveUntil = GetReaderValue<DateTime?>(reader, "EffectiveUntil")
            };
        }

        #endregion
    }
}

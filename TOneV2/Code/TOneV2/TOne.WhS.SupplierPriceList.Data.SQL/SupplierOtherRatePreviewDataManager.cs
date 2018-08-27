using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.Data.SQL;

namespace TOne.WhS.SupplierPriceList.Data.SQL
{
    public class SupplierOtherRatePreviewDataManager : BaseTOneDataManager, ISupplierOtherRatePreviewDataManager
    {
        readonly string[] _columns = { "ProcessInstanceID", "ZoneName", "SystemRate", "SystemRateBED", "SystemRateEED", "ImportedRate", "ImportedRateBED", "RateTypeID", "RateChangeType","IsExcluded" };
        private static Dictionary<string, string> _columnMapper = new Dictionary<string, string>();
       
        public long ProcessInstanceId
        {
            set
            {
                _processInstanceID = value;
            }
        }

        long _processInstanceID;

        public SupplierOtherRatePreviewDataManager()
            : base(GetConnectionStringName("TOneWhS_SPL_DBConnStringKey", "TOneWhS_SPL_DBConnString"))
        {

        }


        public void ApplyPreviewOtherRatesToDB(object preparedOtherRates)
        {
            InsertBulkToTable(preparedOtherRates as BaseBulkInsertInfo);
        }


        public IEnumerable<OtherRatePreview> GetFilteredOtherRatesPreview(SPLPreviewQuery query)
        {
            return GetItemsSP("[TOneWhS_SPL].[sp_SupplierOtherRate_Preview_GetFiltered]", OtherRatePreviewMapper, query.ProcessInstanceId, query.ZoneName, query.OnlyModified,query.IsExcluded);
        }


        object Vanrise.Data.IBulkApplyDataManager<OtherRatePreview>.InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(OtherRatePreview record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^{8}^{9}",
                _processInstanceID,
                record.ZoneName,
                GetRoundedRate(record.SystemRate),
                GetDateTimeForBCP(record.SystemRateBED),
                GetDateTimeForBCP(record.SystemRateEED),
                GetRoundedRate(record.ImportedRate),
                GetDateTimeForBCP(record.ImportedRateBED),
                record.RateTypeId,
                (int)record.ChangeTypeRate,
                 (record.IsExcluded) ? 1 : 0);
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                ColumnNames = _columns,
                TableName = "TOneWhS_SPL.SupplierOtherRate_Preview",
                Stream = streamForBulkInsert,
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = '^',
            };
        }

        private OtherRatePreview OtherRatePreviewMapper(IDataReader reader)
        {
            OtherRatePreview otherRatePreview = new OtherRatePreview
            {
                ZoneName = reader["ZoneName"] as string,
                SystemRate = GetReaderValue<decimal?>(reader, "SystemRate"),
                SystemRateBED = GetReaderValue<DateTime?>(reader, "SystemRateBED"),
                SystemRateEED = GetReaderValue<DateTime?>(reader, "SystemRateEED"),
                ImportedRate = GetReaderValue<decimal?>(reader, "ImportedRate"),
                ImportedRateBED = GetReaderValue<DateTime?>(reader, "ImportedRateBED"),
                RateTypeId = (int) reader["RateTypeID"],
                ChangeTypeRate = (RateChangeType)GetReaderValue<int>(reader, "RateChangeType"),
            };
            return otherRatePreview;
        }

        private decimal? GetRoundedRate(decimal? rate)
        {
            if (rate.HasValue)
                return decimal.Round(rate.Value, 8);
            return null;
        }
        
    }
}

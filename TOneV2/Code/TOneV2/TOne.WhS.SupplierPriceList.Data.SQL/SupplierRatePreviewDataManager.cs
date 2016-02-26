using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.Data.SQL;

namespace TOne.WhS.SupplierPriceList.Data.SQL
{
    public class SupplierRatePreviewDataManager : BaseTOneDataManager, ISupplierRatePreviewDataManager
    {
        readonly string[] _columns = { "ProcessInstanceID", "ZoneName", "ChangeType", "RecentRate", "NewRate", "BED", "EED" };
        private static Dictionary<string, string> _columnMapper = new Dictionary<string, string>();

        public long ProcessInstanceId
        {
            set
            {
                _processInstanceID = value;
            }
        }

        long _processInstanceID;
        public SupplierRatePreviewDataManager()
            : base(GetConnectionStringName("TOneWhS_SPL_DBConnStringKey", "TOneWhS_SPL_DBConnString"))
        {

        }
        static SupplierRatePreviewDataManager()
        {
            _columnMapper.Add("ChangeTypeDecription", "ChangeType");
        }


        public void ApplyPreviewRatesToDB(object preparedZones)
        {
            InsertBulkToTable(preparedZones as BaseBulkInsertInfo);
        }


        
        public Vanrise.Entities.BigResult<Entities.RatePreview> GetRatePreviewFilteredFromTemp(Vanrise.Entities.DataRetrievalInput<Entities.SPLPreviewQuery> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {

                ExecuteNonQuerySP("[TOneWhS_SPL].[sp_SupplierRate_Preview_CreateTempByFiltered]", tempTableName, input.Query.PriceListId);
            };

            if (input.SortByColumnName != null)
                input.SortByColumnName = input.SortByColumnName.Replace("Entity.", "");

            return RetrieveData(input, createTempTableAction, RatePreviewMapper, _columnMapper);
        }

        object Vanrise.Data.IBulkApplyDataManager<RatePreview>.InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream( RatePreview record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}",
                _processInstanceID,
                record.ZoneName,
                (int)record.ChangeType,
                record.RecentRate,
                record.NewRate,
                record.BED,
                record.EED);
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                ColumnNames = _columns,
                TableName = "TOneWhS_SPL.SupplierRate_Preview",
                Stream = streamForBulkInsert,
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = '^',
            };
        }
        private RatePreview RatePreviewMapper(IDataReader reader)
        {
            RatePreview ratePreview = new RatePreview
            {
                ZoneName = reader["ZoneName"] as string,
                ChangeType = (RateChangeType)GetReaderValue<int>(reader, "ChangeType"),
                RecentRate = GetReaderValue<decimal>(reader, "RecentRate"),
                NewRate = GetReaderValue<decimal>(reader, "NewRate"),
                BED = GetReaderValue<DateTime>(reader, "BED"),
                EED = GetReaderValue<DateTime?>(reader, "EED")
            };
            return ratePreview;
        }
 
    }
}

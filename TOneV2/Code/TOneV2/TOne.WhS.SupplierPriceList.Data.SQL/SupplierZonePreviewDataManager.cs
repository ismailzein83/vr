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
    public class SupplierZonePreviewDataManager : BaseTOneDataManager, ISupplierZonePreviewDataManager
    {
        readonly string[] _columns = { "ProcessInstanceID", "CountryID", "ZoneName", "RecentZoneName", "ZoneChangeType", "ZoneBED", "ZoneEED", "CurrentRate", "CurrentRateBED", "CurrentRateEED", "ImportedRate", "ImportedRateBED", "RateChangeType" };
        private static Dictionary<string, string> _columnMapper = new Dictionary<string, string>();
        static SupplierZonePreviewDataManager()
        {
            _columnMapper.Add("ChangeTypeDecription", "ChangeType");
        }

        public long ProcessInstanceId
        {
            set
            {
                _processInstanceID = value;
            }
        }

        long _processInstanceID;

        public SupplierZonePreviewDataManager()
            : base(GetConnectionStringName("TOneWhS_SPL_DBConnStringKey", "TOneWhS_SPL_DBConnString"))
        {

        }


        public void ApplyPreviewZonesToDB(object preparedZones)
        {
            InsertBulkToTable(preparedZones as BaseBulkInsertInfo);
        }


        public Vanrise.Entities.BigResult<Entities.ZoneRatePreviewDetail> GetZonePreviewFilteredFromTemp(Vanrise.Entities.DataRetrievalInput<Entities.SPLPreviewQuery> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {

                ExecuteNonQuerySP("[TOneWhS_SPL].[sp_SupplierZoneRate_Preview_CreateTempByFiltered]", tempTableName, input.Query.ProcessInstanceId, input.Query.CountryId, input.Query.OnlyModified);
            };
            if (input.SortByColumnName != null)
                input.SortByColumnName = input.SortByColumnName.Replace("Entity.", "");

            return RetrieveData(input, createTempTableAction, ZoneRatePreviewMapper, _columnMapper);
        }


        object Vanrise.Data.IBulkApplyDataManager<ZoneRatePreview>.InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(ZoneRatePreview record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^{8}^{9}^{10}^{11}^{12}",
                _processInstanceID,
                record.CountryId,
                record.ZoneName,
                record.RecentZoneName,
                (int)record.ChangeTypeZone,
                record.ZoneBED,
                record.ZoneEED,
                record.CurrentRate,
                record.CurrentRateBED,
                record.CurrentRateEED,
                record.ImportedRate,
                record.ImportedRateBED,
                (int)record.ChangeTypeRate);
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                ColumnNames = _columns,
                TableName = "TOneWhS_SPL.SupplierZoneRate_Preview",
                Stream = streamForBulkInsert,
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = '^',
            };
        }

        private ZoneRatePreviewDetail ZoneRatePreviewMapper(IDataReader reader)
        {
            ZoneRatePreviewDetail zoneRatePreviewDetail = new ZoneRatePreviewDetail
            {
                ProcessInstanceId = (long)reader["ProcessInstanceID"],
                ZoneName = reader["ZoneName"] as string,
                RecentZoneName = reader["RecentZoneName"] as string,
                ChangeTypeZone = (ZoneChangeType)GetReaderValue<int>(reader, "ZoneChangeType"),
                ZoneBED = GetReaderValue<DateTime>(reader, "ZoneBED"),
                ZoneEED = GetReaderValue<DateTime?>(reader, "ZoneEED"),
                CurrentRate = GetReaderValue<decimal>(reader, "CurrentRate"),
                CurrentRateBED = GetReaderValue<DateTime>(reader, "CurrentRateBED"),
                CurrentRateEED = GetReaderValue<DateTime?>(reader, "CurrentRateEED"),
                ImportedRate = GetReaderValue<decimal>(reader, "ImportedRate"),
                ImportedRateBED = GetReaderValue<DateTime?>(reader, "ImportedRateBED"),
                ChangeTypeRate = (RateChangeType)GetReaderValue<int>(reader, "RateChangeType"),
                NewCodes = (int)reader["NewCodes"],
                DeletedCodes = (int)reader["DeletedCodes"],
                CodesMovedFrom = (int)reader["CodesMovedFrom"],
                CodesMovedTo = (int)reader["CodesMovedTo"]

            };
            return zoneRatePreviewDetail;
        }
    }
}

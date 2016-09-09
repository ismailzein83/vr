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
    public class SupplierZonePreviewDataManager : BaseTOneDataManager, ISupplierZonePreviewDataManager
    {
        readonly string[] _columns = { "ProcessInstanceID", "CountryID", "ZoneName", "RecentZoneName", "ZoneChangeType", "ZoneBED", "ZoneEED", "SystemRate", "SystemRateBED", 
                                         "SystemRateEED", "ImportedRate", "ImportedRateBED", "RateChangeType", "SystemZoneServiceIds", "SystemZoneServiceBED", "SystemZoneServiceEED", "ImportedZoneServiceIds", "ImportedZoneServiceBED", "ZoneServiceChangeType" };
       
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


       public IEnumerable<ZoneRatePreviewDetail> GetFilteredZonePreview(SPLPreviewQuery query)
        {
            return GetItemsSP("[TOneWhS_SPL].[sp_SupplierZoneRate_Preview_GetFiltered]", ZoneRatePreviewMapper, query.ProcessInstanceId, query.CountryId, query.OnlyModified);
        }


        object Vanrise.Data.IBulkApplyDataManager<ZoneRatePreview>.InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(ZoneRatePreview record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^{8}^{9}^{10}^{11}^{12}^{13}^{14}^{15}^{16}^{17}^{18}",
                _processInstanceID,
                record.CountryId,
                record.ZoneName,
                record.RecentZoneName,
                (int)record.ChangeTypeZone,
                record.ZoneBED,
                record.ZoneEED,
                GetRoundedRate(record.SystemRate),
                record.SystemRateBED,
                record.SystemRateEED,
                GetRoundedRate(record.ImportedRate),
                record.ImportedRateBED,
                (int)record.ChangeTypeRate,
                 Vanrise.Common.Serializer.Serialize(record.SystemServiceIds),
                record.SystemServicesBED,
                record.SystemServicesEED,
                Vanrise.Common.Serializer.Serialize(record.ImportedServiceIds),
                record.ImportedServicesBED,
                (int)record.ZoneServicesChangeType);

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
                ZoneName = reader["ZoneName"] as string,
                RecentZoneName = reader["RecentZoneName"] as string,
                ChangeTypeZone = (ZoneChangeType)GetReaderValue<int>(reader, "ZoneChangeType"),
                ZoneBED = GetReaderValue<DateTime>(reader, "ZoneBED"),
                ZoneEED = GetReaderValue<DateTime?>(reader, "ZoneEED"),
                SystemRate = GetReaderValue<decimal?>(reader, "SystemRate"),
                SystemRateBED = GetReaderValue<DateTime?>(reader, "SystemRateBED"),
                SystemRateEED = GetReaderValue<DateTime?>(reader, "SystemRateEED"),
                ImportedRate = GetReaderValue<decimal?>(reader, "ImportedRate"),
                ImportedRateBED = GetReaderValue<DateTime?>(reader, "ImportedRateBED"),
                ChangeTypeRate = (RateChangeType)GetReaderValue<int>(reader, "RateChangeType"),
                NewCodes = GetReaderValue<int>(reader,"NewCodes"),
                DeletedCodes = GetReaderValue<int>(reader,"DeletedCodes"),
                CodesMovedFrom = GetReaderValue<int>(reader,"CodesMovedFrom"),
                CodesMovedTo = GetReaderValue<int>(reader,"CodesMovedTo")

            };
            return zoneRatePreviewDetail;
        }

        private decimal? GetRoundedRate(decimal? rate)
        {
            if (rate.HasValue)
                return decimal.Round(rate.Value, 8);
            return null;
        }

        
    }
}

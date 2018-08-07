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
    public class SupplierZoneServicePreviewDataManager : BaseTOneDataManager, ISupplierZoneServicePreviewDataManager
    {
        readonly string[] _columns = { "ProcessInstanceID", "ZoneName", "SystemZoneServiceIds", "SystemZoneServiceBED", "SystemZoneServiceEED", "ImportedZoneServiceIds", "ImportedZoneServiceBED", "ZoneServiceChangeType" };
        public long ProcessInstanceId
        {
            set
            {
                _processInstanceID = value;
            }
        }

        long _processInstanceID;

        public SupplierZoneServicePreviewDataManager()
            : base(GetConnectionStringName("TOneWhS_SPL_DBConnStringKey", "TOneWhS_SPL_DBConnString"))
        {

        }


        public void ApplyPreviewZonesServicesToDB(object preparedZonesServices)
        {
            InsertBulkToTable(preparedZonesServices as BaseBulkInsertInfo);
        }


        public IEnumerable<ZoneServicePreview> GetFilteredZonesServicesPreview(SPLPreviewQuery query)
        {
            return GetItemsSP("[TOneWhS_SPL].[sp_SupplierZonesServices_Preview_GetFiltered]", ZoneServicePreviewMapper, query.ProcessInstanceId, query.ZoneName, query.OnlyModified,query.IsExcluded);
        }


        object Vanrise.Data.IBulkApplyDataManager<ZoneServicePreview>.InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(ZoneServicePreview record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}",
                _processInstanceID,
                record.ZoneName,
                Vanrise.Common.Serializer.Serialize(record.SystemServiceIds, true),
                GetDateTimeForBCP(record.SystemServicesBED),
                GetDateTimeForBCP(record.SystemServicesEED),
                Vanrise.Common.Serializer.Serialize(record.ImportedServiceIds, true),
                GetDateTimeForBCP(record.ImportedServicesBED),
                (int)record.ZoneServicesChangeType);
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                ColumnNames = _columns,
                TableName = "TOneWhS_SPL.SupplierZoneService_Preview",
                Stream = streamForBulkInsert,
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = '^',
            };
        }

        private ZoneServicePreview ZoneServicePreviewMapper(IDataReader reader)
        {
            ZoneServicePreview zoneServicePreview = new ZoneServicePreview
            {
                ZoneName = reader["ZoneName"] as string,
                ImportedServiceIds = Vanrise.Common.Serializer.Deserialize<List<int>>(reader["ImportedZoneServiceIds"] as string),
                ImportedServicesBED = GetReaderValue<DateTime?>(reader, "ImportedZoneServiceBED"),
                SystemServiceIds = Vanrise.Common.Serializer.Deserialize<List<int>>(reader["SystemZoneServiceIds"] as string),
                SystemServicesBED = GetReaderValue<DateTime?>(reader, "SystemZoneServiceBED"),
                SystemServicesEED = GetReaderValue<DateTime?>(reader, "SystemZoneServiceEED"),
                ZoneServicesChangeType = (ZoneServiceChangeType)GetReaderValue<int>(reader, "ZoneServiceChangeType"),
            };
            return zoneServicePreview;
        }




    }
}

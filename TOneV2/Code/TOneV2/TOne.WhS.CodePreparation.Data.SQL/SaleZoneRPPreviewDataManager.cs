using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.CodePreparation.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.CodePreparation.Data.SQL
{
    public class SaleZoneRoutingProductPreviewDataManager : BaseTOneDataManager, ISaleZoneRoutingProductPreviewDataManager
    {
        readonly string[] _columns = { "ProcessInstanceID", "ZoneName", "OwnerType", "OwnerID", "RoutingProductID", "BED", "EED", "ChangeType" };

        public long ProcessInstanceId
        {
            set
            {
                _processInstanceID = value;
            }
        }

        long _processInstanceID;

        public SaleZoneRoutingProductPreviewDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        public void ApplyPreviewZonesRoutingProductsToDB(object preparedZonesRoutingProducts)
        {
            InsertBulkToTable(preparedZonesRoutingProducts as BaseBulkInsertInfo);
        }

        public IEnumerable<ZoneRoutingProductPreview> GetFilteredZonesRoutingProductsPreview(SPLPreviewQuery query)
        {
            return GetItemsSP("[TOneWhs_CP].[sp_SaleZoneRoutingProduct_Preview_GetFiltered]", ZoneRoutingProductPreviewMapper, query.ProcessInstanceId, query.ZoneName,query.OnlyModified);
        }


        object Vanrise.Data.IBulkApplyDataManager<ZoneRoutingProductPreview>.InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(ZoneRoutingProductPreview record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}",
                _processInstanceID,
                record.ZoneName,
                (int)record.OwnerType,
                record.OwnerId,
                record.RoutingProductId,
               GetDateTimeForBCP(record.BED),
               GetDateTimeForBCP(record.EED),
                (int)record.ChangeType);

        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                ColumnNames = _columns,
                TableName = "TOneWhS_CP.SaleZoneRoutingProduct_Preview",
                Stream = streamForBulkInsert,
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = '^',
            };
        }


        private ZoneRoutingProductPreview ZoneRoutingProductPreviewMapper(IDataReader reader)
        {
            ZoneRoutingProductPreview zoneRoutingProductPreview = new ZoneRoutingProductPreview
            {
                ZoneName = GetReaderValue<string>(reader, "ZoneName"),
                OwnerType = (SalePriceListOwnerType)GetReaderValue<byte>(reader, "OwnerType"),
                OwnerId = (int)reader["OwnerID"],
                RoutingProductId = (int)reader["RoutingProductID"],
                BED = (DateTime)reader["BED"],
                EED = GetReaderValue<DateTime?>(reader, "EED"),
                ChangeType = (ZoneRoutingProductChangeType)GetReaderValue<int>(reader, "ChangeType"),
            };
            return zoneRoutingProductPreview;
        }
    }
}

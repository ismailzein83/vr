using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.Data.SQL;

namespace TOne.WhS.CodePreparation.Data.SQL
{
    public class NewSaleZoneRoutingProductDataManager : BaseTOneDataManager, INewSaleZoneRoutingProductDataManager
    {
        public NewSaleZoneRoutingProductDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        readonly string[] _columns = { "ID","ProcessInstanceID", "ZoneID", "OwnerType", "OwnerID", "RoutingProductID", "BED", "EED" };


        public long ProcessInstanceId
        {
            set
            {
                _processInstanceID = value;
            }
        }
        long _processInstanceID;
      
        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(AddedZoneRoutingProduct record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}",
                       record.SaleEntityRoutingProductId,
                       _processInstanceID,
                       record.AddedZone.ZoneId,
                       (int)record.OwnerType,
                       record.OwnerId,
                       record.RoutingProductId,
                       GetDateTimeForBCP(record.BED),
                       GetDateTimeForBCP(record.EED));
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                ColumnNames = _columns,
                TableName = "TOneWhS_BE.CP_SaleZoneRoutingProduct_New",
                Stream = streamForBulkInsert,
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = '^',
            };
        }

        public void ApplyNewZonesRoutingProductsToDB(object preparedZonesRoutingProducts)
        {
            InsertBulkToTable(preparedZonesRoutingProducts as BaseBulkInsertInfo);

        }

      
    }
}

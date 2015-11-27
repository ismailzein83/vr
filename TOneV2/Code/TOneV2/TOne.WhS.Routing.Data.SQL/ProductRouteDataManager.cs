using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace TOne.WhS.Routing.Data.SQL
{
    public class ProductRouteDataManager : RoutingDataManager, IProductRouteDataManager
    {
        public void ApplyProductRouteForDB(object preparedProductRoute)
        {
            InsertBulkToTable(preparedProductRoute as BaseBulkInsertInfo);
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "[dbo].[ProductRoute]",
                Stream = streamForBulkInsert,
                TabLock = true,
                KeepIdentity = false,
                FieldSeparator = '^',
            };
        }

        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(Entities.RPRoute record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}", record.RoutingProductId, record.SaleZoneId, record.ExecutedRuleId, Vanrise.Common.Serializer.Serialize(record.OptionsDetailsBySupplier, true), Vanrise.Common.Serializer.Serialize(record.RPOptionsByPolicy, true), record.IsBlocked ? 1 : 0);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace TOne.WhS.Routing.Data.SQL
{
    public class CustomerRouteDataManager : RoutingDataManager, ICustomerRouteDataManager
    {
        public void ApplyCustomerRouteForDB(object preparedCustomerRoute)
        {
            InsertBulkToTable(preparedCustomerRoute as BaseBulkInsertInfo);
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "[dbo].[CustomerRoute]",
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

        public void WriteRecordToStream(Entities.CustomerRoute record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}", record.CustomerId, record.Code, record.SaleZoneId, 
                record.Rate, Vanrise.Common.Serializer.Serialize(record.Options, true));
        }
    }
}

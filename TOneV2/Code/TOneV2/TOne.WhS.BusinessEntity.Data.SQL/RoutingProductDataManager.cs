using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class RoutingProductDataManager : BaseSQLDataManager, IRoutingProductDataManager
    {
        //public Vanrise.Entities.BigResult<Entities.RoutingProduct> GetFilteredRoutingProducts(Vanrise.Entities.DataRetrievalInput<object> input)
        //{
        //    Action<string> createTempTableAction = (tempTableName) =>
        //    {
        //        ExecuteNonQuerySP("integration.sp_DataSource_CreateTemp", tempTableName);
        //    };

        //    return RetrieveData(input, createTempTableAction, DataSourceMapper, _columnMapper);
        //}

        public bool Insert(Entities.RoutingProduct routingProduct, out int insertedId)
        {
            object routingProductId;

            int recordesEffected = ExecuteNonQuerySP("TOneWhS_BE.sp_RoutingProduct_Insert", out routingProductId, routingProduct.Name, routingProduct.SaleZonePackageId,
                Vanrise.Common.Serializer.Serialize(routingProduct.Settings));

            insertedId = (int)routingProductId;
            return (recordesEffected > 0);
        }

        public bool Update(Entities.RoutingProduct routingProduct)
        {
            int recordesEffected = ExecuteNonQuerySP("TOneWhS_BE.sp_RoutingProduct_Update", routingProduct.RoutingProductId, routingProduct.Name, routingProduct.SaleZonePackageId,
                Vanrise.Common.Serializer.Serialize(routingProduct.Settings));
            return (recordesEffected > 0);
        }
    }
}

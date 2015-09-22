using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class RoutingProductDataManager : BaseSQLDataManager, IRoutingProductDataManager
    {
        private static Dictionary<string, string> _columnMapper = new Dictionary<string, string>();

        static RoutingProductDataManager()
        {
            _columnMapper.Add("RoutingProductId", "ID");
        }

        public RoutingProductDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        public Vanrise.Entities.BigResult<Entities.RoutingProduct> GetFilteredRoutingProducts(Vanrise.Entities.DataRetrievalInput<Entities.RoutingProductQuery> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {
                string saleZonePackageIdsParam = null;
                if (input.Query.SaleZonePackageIds != null)
                    saleZonePackageIdsParam = string.Join(",", input.Query.SaleZonePackageIds);
                
                ExecuteNonQuerySP("TOneWhS_BE.sp_RoutingProduct_CreateTempByFiltered", tempTableName, input.Query.Name, saleZonePackageIdsParam);
            };

            return RetrieveData(input, createTempTableAction, RoutingProductMapper, _columnMapper);
        }

        public Entities.RoutingProduct GetRoutingProduct(int routingProductId)
        {
            return GetItemSP("TOneWhS_BE.sp_RoutingProduct_Get", RoutingProductMapper, routingProductId);
        }

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

        Entities.RoutingProduct RoutingProductMapper(IDataReader reader)
        {
            Entities.RoutingProduct routingProduct = new Entities.RoutingProduct
            {
                RoutingProductId = (int)reader["ID"],
                Name = reader["Name"] as string,
                SaleZonePackageId = (int)reader["SaleZonePackageID"],
                Settings = Vanrise.Common.Serializer.Deserialize<Entities.RoutingProductSettings>(reader["Settings"] as string)
            };

            return routingProduct;
        }
    }
}

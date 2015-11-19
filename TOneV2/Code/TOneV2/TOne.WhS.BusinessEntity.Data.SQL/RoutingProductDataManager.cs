using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities.RatePlanning;
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
                string sellingNumberPlanIdsParam = null;
                if (input.Query.SellingNumberPlanIds != null)
                    sellingNumberPlanIdsParam = string.Join(",", input.Query.SellingNumberPlanIds);
                
                ExecuteNonQuerySP("TOneWhS_BE.sp_RoutingProduct_CreateTempByFiltered", tempTableName, input.Query.Name, sellingNumberPlanIdsParam);
            };

            return RetrieveData(input, createTempTableAction, RoutingProductMapper, _columnMapper);
        }

        public Entities.RoutingProduct GetRoutingProduct(int routingProductId)
        {
            return GetItemSP("TOneWhS_BE.sp_RoutingProduct_Get", RoutingProductMapper, routingProductId);
        }

        public IEnumerable<Entities.RoutingProduct> GetRoutingProducts()
        {
            return GetItemsSP("TOneWhS_BE.sp_RoutingProduct_GetAll", RoutingProductMapper);
        }

        public bool Insert(Entities.RoutingProduct routingProduct, out int insertedId)
        {
            object routingProductId;

            int recordsEffected = ExecuteNonQuerySP("TOneWhS_BE.sp_RoutingProduct_Insert", out routingProductId, routingProduct.Name, routingProduct.SellingNumberPlanId,
                Vanrise.Common.Serializer.Serialize(routingProduct.Settings));

            insertedId = (int)routingProductId;
            return (recordsEffected > 0);
        }

        public bool Update(Entities.RoutingProduct routingProduct)
        {
            int recordsEffected = ExecuteNonQuerySP("TOneWhS_BE.sp_RoutingProduct_Update", routingProduct.RoutingProductId, routingProduct.Name, routingProduct.SellingNumberPlanId,
                Vanrise.Common.Serializer.Serialize(routingProduct.Settings));
            return (recordsEffected > 0);
        }

        public bool Delete(int routingProductId)
        {
            int recordesEffected = ExecuteNonQuerySP("TOneWhS_BE.sp_RoutingProduct_Delete", routingProductId);
            return (recordesEffected > 0);
        }

        Entities.RoutingProduct RoutingProductMapper(IDataReader reader)
        {
            Entities.RoutingProduct routingProduct = new Entities.RoutingProduct
            {
                RoutingProductId = (int)reader["ID"],
                Name = reader["Name"] as string,
                SellingNumberPlanId = (int)reader["SellingNumberPlanID"],
                Settings = Vanrise.Common.Serializer.Deserialize<Entities.RoutingProductSettings>(reader["Settings"] as string)
            };

            return routingProduct;
        }

        //Entities.RoutingProductInfo RoutingProductInfoMapper(IDataReader reader)
        //{
        //    Entities.RoutingProductInfo routingProductInfo = new Entities.RoutingProductInfo
        //    {
        //        RoutingProductId = (int)reader["ID"],
        //        Name = reader["Name"] as string,
        //        SellingNumberPlanId = (int)reader["SellingNumberPlanID"]
        //    };

        //    return routingProductInfo;
        //}

        public bool AreRoutingProductsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("TOneWhS_BE.RoutingProduct", ref updateHandle);
        }

        //public List<Entities.RoutingProductInfo> GetRoutingProductsInfoBySellingNumberPlan(int sellingNumberPlan)
        //{
        //    return GetItemsSP("TOneWhS_BE.sp_RoutingProduct_GetBySellingNumberPlan", RoutingProductInfoMapper, sellingNumberPlan);
        //}
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class PricingProductDataManager : BaseSQLDataManager, IPricingProductDataManager
    {
        private static Dictionary<string, string> _columnMapper = new Dictionary<string, string>();

        static PricingProductDataManager()
        {
            _columnMapper.Add("PricingProductId", "ID");
        }

        public PricingProductDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        public Vanrise.Entities.BigResult<PricingProduct> GetFilteredPricingProducts(Vanrise.Entities.DataRetrievalInput<PricingProductQuery> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {
                //string saleZonePackageIdsParam = null;
                //if (input.Query.SaleZonePackageIds != null)
                //    saleZonePackageIdsParam = string.Join(",", input.Query.SaleZonePackageIds);

                ExecuteNonQuerySP("TOneWhS_BE.sp_PricingProduct_CreateTempByFiltered", tempTableName, input.Query.Name);
            };

            return RetrieveData(input, createTempTableAction, PricingProductMapper, _columnMapper);
        }

        public Entities.PricingProduct GetPricingProduct(int pricingProductId)
        {
            return GetItemSP("TOneWhS_BE.sp_PricingProduct_Get", PricingProductMapper, pricingProductId);
        }

        public bool Insert(PricingProduct pricingProduct, out int insertedId)
        {
            object pricingProductId;

            int recordsEffected = ExecuteNonQuerySP("TOneWhS_BE.sp_PricingProduct_Insert", out pricingProductId, pricingProduct.Name,pricingProduct.DefaultRoutingProductId, pricingProduct.SaleZonePackageId,
                Vanrise.Common.Serializer.Serialize(pricingProduct.Settings));

            insertedId = (int)pricingProductId;
            return (recordsEffected > 0);
        }

        public bool Update(PricingProduct pricingProduct)
        {
            int recordsEffected = ExecuteNonQuerySP("TOneWhS_BE.sp_PricingProduct_Update", pricingProduct.PricingProductId, pricingProduct.Name, pricingProduct.DefaultRoutingProductId, pricingProduct.SaleZonePackageId,
                Vanrise.Common.Serializer.Serialize(pricingProduct.Settings));
            return (recordsEffected > 0);
        }

        public bool Delete(int pricingProductId)
        {
            int recordesEffected = ExecuteNonQuerySP("TOneWhS_BE.sp_PricingProduct_Delete", pricingProductId);
            return (recordesEffected > 0);
        }

        PricingProduct PricingProductMapper(IDataReader reader)
        {
            PricingProduct pricingProduct = new PricingProduct
            {
                PricingProductId = (int)reader["ID"],
                Name = reader["Name"] as string,
                DefaultRoutingProductId = (int)reader["DefaultRoutingProductID"],
                SaleZonePackageId = (int)reader["SaleZonePackageID"],
                Settings = ((reader["Settings"] as string) != null) ? Vanrise.Common.Serializer.Deserialize<PricingProductSettings>(reader["Settings"] as string) : null,
            };

            return pricingProduct;
        }
    }
}

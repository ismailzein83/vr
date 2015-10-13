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
     
        public PricingProductDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

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

        PricingProductDetail PricingProductDetailMapper(IDataReader reader)
        {
            PricingProductDetail pricingProductDetail = new PricingProductDetail
            {
                PricingProductId = (int)reader["ID"],
                Name = reader["Name"] as string,
                DefaultRoutingProductId = GetReaderValue<int?>(reader, "DefaultRoutingProductID"),
                SaleZonePackageId = (int)reader["SaleZonePackageID"],
                Settings = ((reader["Settings"] as string) != null) ? Vanrise.Common.Serializer.Deserialize<PricingProductSettings>(reader["Settings"] as string) : null,
                SaleZonePackageName = reader["SaleZonePackageName"] as string,
                DefaultRoutingProductName = reader["DefaultRoutingProductName"] as string,
            };
            return pricingProductDetail;
        }


        public bool ArePricingProductsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("TOneWhS_BE.PricingProduct", ref updateHandle);
        }


        public List<PricingProductDetail> GetPricingProducts()
        {
            return GetItemsSP("TOneWhS_BE.sp_PricingProduct_GetAll", PricingProductDetailMapper);
        }
    }
}

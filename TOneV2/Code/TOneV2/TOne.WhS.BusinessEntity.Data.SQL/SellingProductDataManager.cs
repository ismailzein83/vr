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
    public class SellingProductDataManager : BaseSQLDataManager, ISellingProductDataManager
    {
     
        public SellingProductDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        public bool Insert(SellingProduct sellingProduct, out int insertedId)
        {
            object sellingProductId;

            int recordsEffected = ExecuteNonQuerySP("TOneWhS_BE.sp_SellingProduct_Insert", out sellingProductId, sellingProduct.Name, sellingProduct.DefaultRoutingProductId, sellingProduct.SellingNumberPlanId,
                Vanrise.Common.Serializer.Serialize(sellingProduct.Settings));

            insertedId = (int)sellingProductId;
            return (recordsEffected > 0);
        }

        public bool Update(SellingProduct sellingProduct)
        {
            int recordsEffected = ExecuteNonQuerySP("TOneWhS_BE.sp_SellingProduct_Update", sellingProduct.SellingProductId, sellingProduct.Name, sellingProduct.DefaultRoutingProductId, sellingProduct.SellingNumberPlanId,
                Vanrise.Common.Serializer.Serialize(sellingProduct.Settings));
            return (recordsEffected > 0);
        }

        public bool Delete(int sellingProductId)
        {
            int recordesEffected = ExecuteNonQuerySP("TOneWhS_BE.sp_SellingProduct_Delete", sellingProductId);
            return (recordesEffected > 0);
        }

        SellingProduct SellingProductMapper(IDataReader reader)
        {
            SellingProduct sellingProductDetail = new SellingProduct
            {
                SellingProductId = (int)reader["ID"],
                Name = reader["Name"] as string,
                DefaultRoutingProductId = GetReaderValue<int?>(reader, "DefaultRoutingProductID"),
                SellingNumberPlanId = (int)reader["SellingNumberPlanID"],
                Settings = ((reader["Settings"] as string) != null) ? Vanrise.Common.Serializer.Deserialize<SellingProductSettings>(reader["Settings"] as string) : null,
            };
            return sellingProductDetail;
        }


        public bool AreSellingProductsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("TOneWhS_BE.SellingProduct", ref updateHandle);
        }


        public List<SellingProduct> GetSellingProducts()
        {
            return GetItemsSP("TOneWhS_BE.sp_SellingProduct_GetAll", SellingProductMapper);
        }

    }
}

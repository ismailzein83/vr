using System;
using System.Collections.Generic;
using System.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class SellingProductDataManager : BaseSQLDataManager, ISellingProductDataManager
    {

        #region ctor/Local Variables
        public SellingProductDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        #endregion

        #region Public Methods
        public bool Insert(SellingProduct sellingProduct, out int insertedId)
        {
            object sellingProductId;

            int recordsEffected = ExecuteNonQuerySP("TOneWhS_BE.sp_SellingProduct_Insert", out sellingProductId, sellingProduct.Name, sellingProduct.SellingNumberPlanId,
                Vanrise.Common.Serializer.Serialize(sellingProduct.Settings), sellingProduct.CreatedBy, sellingProduct.LastModifiedBy);

            insertedId = (int)sellingProductId;
            return (recordsEffected > 0);
        }
        public bool Update(SellingProductToEdit sellingProduct)
        {
            var settings = (sellingProduct.Settings != null) ? Vanrise.Common.Serializer.Serialize(sellingProduct.Settings) : null;
            int recordsEffected = ExecuteNonQuerySP("TOneWhS_BE.sp_SellingProduct_Update", sellingProduct.SellingProductId, sellingProduct.Name, settings, sellingProduct.LastModifiedBy);
            return (recordsEffected > 0);
        }
        public bool AreSellingProductsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("TOneWhS_BE.SellingProduct", ref updateHandle);
        }
        public List<SellingProduct> GetSellingProducts()
        {
            return GetItemsSP("TOneWhS_BE.sp_SellingProduct_GetAll", SellingProductMapper);
        }
        #endregion

        #region Private Methods
        #endregion

        #region Mappers
        SellingProduct SellingProductMapper(IDataReader reader)
        {
            SellingProduct sellingProductDetail = new SellingProduct
            {
                SellingProductId = (int)reader["ID"],
                Name = reader["Name"] as string,
                SellingNumberPlanId = (int)reader["SellingNumberPlanID"],
                Settings = ((reader["Settings"] as string) != null) ? Vanrise.Common.Serializer.Deserialize<SellingProductSettings>(reader["Settings"] as string) : null,
                CreatedTime = GetReaderValue<DateTime?>(reader, "CreatedTime"),
                CreatedBy = GetReaderValue<int?>(reader, "CreatedBy"),
                LastModifiedBy = GetReaderValue<int?>(reader, "LastModifiedBy"),
                LastModifiedTime = GetReaderValue<DateTime?>(reader, "LastModifiedTime")
            };
            return sellingProductDetail;
        }
        #endregion

    }
}

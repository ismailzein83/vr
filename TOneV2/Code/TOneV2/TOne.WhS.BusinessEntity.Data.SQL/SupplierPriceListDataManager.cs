using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class SupplierPriceListDataManager : BaseTOneDataManager, ISupplierPriceListDataManager
    {
        public SupplierPriceListDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }


        public Entities.SupplierPriceList GetPriceList(int priceListId)
        {
            return GetItemSP("TOneWhS_BE.sp_SupplierPriceList_Get", SupplierPriceListMapper, priceListId);
        }
        SupplierPriceList SupplierPriceListMapper(IDataReader reader)
        {
            SupplierPriceList supplierPriceList = new SupplierPriceList
            {
                SupplierId = (int)reader["SupplierID"],
                CurrencyId = (int)reader["CurrencyID"],
                PriceListId = (int)reader["ID"],
                FileId = (long)reader["FileID"]
            };
            return supplierPriceList;
        }



        public List<SupplierPriceList> GetPriceLists()
        {
            return GetItemsSP("TOneWhS_BE.sp_SupplierPriceList_GetAll", SupplierPriceListMapper);
        }

        public bool ArGetPriceListsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("TOneWhS_BE.SupplierPriceList", ref updateHandle);
        }
    }
}

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
    public class SalePriceListDataManager : BaseSQLDataManager, ISalePriceListDataManager
    {
        public SalePriceListDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }
       SalePriceList SalePriceListMapper(IDataReader reader)
        {
            SalePriceList salePriceList = new SalePriceList
            {
                OwnerId = (int)reader["OwnerID"],
                CurrencyId = (int)reader["CurrencyID"],
                PriceListId = (int)reader["ID"],
                OwnerType = (Entities.SalePriceListOwnerType)GetReaderValue<int>(reader, "OwnerType"),
            };
            return salePriceList;
        }


        public List<Entities.SalePriceList> GetPriceLists()
        {
            return GetItemsSP("TOneWhS_BE.sp_SalePriceList_GetAll", SalePriceListMapper);
        }

        public bool ArGetSalePriceListsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("TOneWhS_BE.SalePriceList", ref updateHandle);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class SupplierPriceListDataManager : BaseTOneDataManager, ISupplierPriceListDataManager
    {
        public SupplierPriceListDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }
        public bool AddSupplierPriceList(int supplierAccountId, out int supplierPriceListId)
        {
            object priceListID;

            int recordesEffected = ExecuteNonQuerySP("TOneWhS_BE.sp_SupplierPriceList_Insert",  out priceListID,supplierAccountId);
            supplierPriceListId = (int)priceListID;
            return (recordesEffected > 0);
        }
    }
}

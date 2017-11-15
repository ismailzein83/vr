using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;

namespace TOne.WhS.SupplierPriceList.Data.SQL
{
    public class SupplierPriceListDataManager : BaseTOneDataManager, ISupplierPriceListDataManager

    {
        public SupplierPriceListDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }
        public bool AddSupplierPriceList(int supplierAccountId, int? currencyId, out int supplierPriceListId)
        {
            object priceListID;

            int recordesEffected = ExecuteNonQuerySP("TOneWhS_BE.sp_SupplierPriceList_Insert", out priceListID, supplierAccountId, currencyId);
            supplierPriceListId = (int)priceListID;
            return (recordesEffected > 0);
        }

        public bool CleanTemporaryTables(long processInstanceId)
        {
            int recordesEffected = ExecuteNonQuerySP("TOneWhS_SPL.sp_CleanTemporaryTablesSPL", processInstanceId);
            return (recordesEffected > 0);
        }
        public bool AddPriceListAndSyncImportedDataWithDB(int priceListId, long processInstanceId, long splStateBackupId, int supplierId, int currencyId, long fileId, DateTime effectiveOn, int userId)
        {
            int recordesEffected = ExecuteNonQuerySP("TOneWhS_BE.sp_SupplierPriceList_SyncWithImportedData", priceListId, processInstanceId, splStateBackupId, supplierId, currencyId, fileId, effectiveOn, userId);
            return (recordesEffected > 0);
        }
    }
}

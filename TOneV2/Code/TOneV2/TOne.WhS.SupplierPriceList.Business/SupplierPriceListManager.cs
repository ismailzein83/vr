using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Data;
using Vanrise.Common.Business;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class SupplierPriceListManager
    {
        public bool AddSupplierPriceList(int supplierAccountId, int? currencyId, out int supplierPriceListId)
        {
            ISupplierPriceListDataManager dataManager = SupPLDataManagerFactory.GetDataManager<ISupplierPriceListDataManager>();
            return dataManager.AddSupplierPriceList(supplierAccountId,currencyId, out supplierPriceListId);
        }

        public long ReserveIDRange(int numberOfIDs)
        {
            long startingId;
            IDManager.Instance.ReserveIDRange(this.GetType(), numberOfIDs, out startingId);
            return startingId;
        }

        public bool AddPriceListAndSyncImportedDataWithDB(int priceListId, int supplierId, int currencyId)
        {
            ISupplierPriceListDataManager dataManager = SupPLDataManagerFactory.GetDataManager<ISupplierPriceListDataManager>();
            return dataManager.AddPriceListAndSyncImportedDataWithDB(priceListId, supplierId, currencyId);
        }
    }
}

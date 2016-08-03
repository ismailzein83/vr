using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Data;
using TOne.WhS.SupplierPriceList.Entities;
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
       

        public bool AddPriceListAndSyncImportedDataWithDB(int priceListId, long processInstanceId, int supplierId, int currencyId, long fileId, DateTime effectiveOn)
        {
            ISupplierPriceListDataManager dataManager = SupPLDataManagerFactory.GetDataManager<ISupplierPriceListDataManager>();
            return dataManager.AddPriceListAndSyncImportedDataWithDB(priceListId, processInstanceId, supplierId, currencyId, fileId, effectiveOn);
        }

       

        public bool ValidateSupplierPriceList(Entities.SupplierPriceListInput input)
        {
            SupplierPriceListExecutionContext context = new SupplierPriceListExecutionContext();
            context.InputFileId = input.InputFileId;
            return true;
        }


    }
}

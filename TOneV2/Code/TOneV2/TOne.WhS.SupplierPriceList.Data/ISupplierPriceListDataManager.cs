﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.SupplierPriceList.Data
{
    public interface ISupplierPriceListDataManager : IDataManager
    {
        bool AddSupplierPriceList(int supplierAccountId, int? currencyId, out int supplierPriceListId);

        bool AddPriceListAndSyncImportedDataWithDB(int priceListId, int supplierId, int currencyId, long fileId);
    }
}

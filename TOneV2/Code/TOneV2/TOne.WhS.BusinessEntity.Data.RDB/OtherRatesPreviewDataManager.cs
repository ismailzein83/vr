using System;
using Vanrise.Common;
using Vanrise.Data.RDB;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;
namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class OtherRatesPreviewDataManager : IOtherRatesPreviewDataManager
    {
        #region IOtherRatesPreviewDataManager Members

        public IEnumerable<SalePricelistRateChange> GetFilteredRatesPreviewByProcessInstanceId(int processInstanceId, string zoneName, int customerId)
        {
            SalePricelistRateChangeNewDataManager salePricelistRateChangeNewDataManager = new SalePricelistRateChangeNewDataManager();
            return salePricelistRateChangeNewDataManager.GetFilteredRatesPreviewByProcessInstanceId(processInstanceId, zoneName, customerId);
        }

        public IEnumerable<SalePricelistRateChange> GetFilteredRatesPreviewByPriceListId(int pricelistId, string zoneName)
        {
            SalePricelistRateChangeDataManager salePricelistRateChangeDataManager = new SalePricelistRateChangeDataManager();
            return salePricelistRateChangeDataManager.GetFilteredRatesPreviewByPriceListId(pricelistId, zoneName);
        }

        #endregion
    }
}

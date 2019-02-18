using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.SMSBusinessEntity.Data;
using TOne.WhS.SMSBusinessEntity.Entities;
using Vanrise.Common.Business;

namespace TOne.WhS.SMSBusinessEntity.Business
{
    public class SupplierSMSPriceListManager
    {
        ISupplierSMSPriceListDataManager _supplierSMSPriceListDataManager = SMSBEDataFactory.GetDataManager<ISupplierSMSPriceListDataManager>();

        public SupplierSMSPriceList CreateSupplierSMSPriceList(int supplierID, int currencyID, DateTime effectiveDate, long processInstanceID, int userID)
        {
            return new SupplierSMSPriceList()
            {
                ID = ReserveIdRange(1),
                SupplierID = supplierID,
                CurrencyID = currencyID,
                EffectiveOn = effectiveDate,
                UserID = userID,
                ProcessInstanceID = processInstanceID
            };
        }

        public Dictionary<long, SupplierSMSPriceList> GetCachedSupplierSMSPriceLists()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedSupplierSMSPriceLists", () =>
            {
                var priceLists = _supplierSMSPriceListDataManager.GetSupplierSMSPriceLists();

                return priceLists != null ? priceLists.ToDictionary(item => item.ID, item => item) : null;
            });
        }

        internal class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ISupplierSMSPriceListDataManager _supplierSMSPriceListDataManager = SMSBEDataFactory.GetDataManager<ISupplierSMSPriceListDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _supplierSMSPriceListDataManager.AreSupplierSMSPriceListUpdated(ref _updateHandle);
            }
        }

        private long ReserveIdRange(int numberOfIds)
        {
            long startingId;
            IDManager.Instance.ReserveIDRange(this.GetType(), numberOfIds, out startingId);
            return startingId;
        }
    }
}
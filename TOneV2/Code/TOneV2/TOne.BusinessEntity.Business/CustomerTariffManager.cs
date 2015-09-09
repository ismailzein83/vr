using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Data;
using TOne.BusinessEntity.Entities;
using TOne.Caching;
using TOne.Entities;

namespace TOne.BusinessEntity.Business
{
    public class CustomerTariffManager
    {
         TOneCacheManager _cacheManager;
         public CustomerTariffManager()
         {
         }
         public CustomerTariffManager(TOneCacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }
        public Vanrise.Entities.IDataRetrievalResult<CustomerTariff> GetFilteredCustomerTariffs(Vanrise.Entities.DataRetrievalInput<CustomerTariffQuery> input)
        {
            ICustomerTariffDataManager dataManager = BEDataManagerFactory.GetDataManager<ICustomerTariffDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetFilteredCustomerTariffs(input));
        }
        public CustomerTariff GetCustomerTariff(string customerId, int zoneId, DateTime when)
        {

            TariffsByCustomer tariffsByCustomer = _cacheManager.GetOrCreateObject(String.Format("GetEffectiveCustomerTariff_{0}_{1:ddMMMyy}", customerId, when.Date),
             CacheObjectType.Pricing,
             () =>
             {
                 return GetCustomerTariffsFromDB(when);
             });

            CustomerTariffsByZone tariffByZoneObject;
            if (tariffsByCustomer.TryGetValue(customerId, out tariffByZoneObject))
            {
                CustomerTariff tariff;
                tariffByZoneObject.TryGetValue(zoneId, out tariff);
                return tariff;
            }
            return null;
        }
        private static TariffsByCustomer GetCustomerTariffsFromDB(DateTime when)
        {
            ICustomerTariffDataManager dataManager = BEDataManagerFactory.GetDataManager<ICustomerTariffDataManager>();
            List<CustomerTariff> customerTariffs = dataManager.GetSaleTariffs(when);
            TariffsByCustomer tariffsByCustomer = new TariffsByCustomer();
            foreach (CustomerTariff tariff in customerTariffs)
            {
                CustomerTariffsByZone customerTariffsByZone;
                if (!tariffsByCustomer.TryGetValue(tariff.CustomerID, out customerTariffsByZone))
                {
                    customerTariffsByZone = new CustomerTariffsByZone();
                    tariffsByCustomer.Add(tariff.CustomerID, customerTariffsByZone);
                }
                if (!customerTariffsByZone.ContainsKey(tariff.ZoneID))
                    customerTariffsByZone.Add(tariff.ZoneID, tariff);
            }
            return tariffsByCustomer;
        }
    }
}

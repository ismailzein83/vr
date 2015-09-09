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
    public class CustomerTODManager : BaseTODManager<CustomerTODConsiderationInfo>
    {
          TOneCacheManager _cacheManager;
         public CustomerTODManager()
         {
         }
         public CustomerTODManager(TOneCacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }
        public Vanrise.Entities.IDataRetrievalResult<CustomerTODConsiderationInfo> GetCustomerTODFiltered(Vanrise.Entities.DataRetrievalInput<TODQuery> input)
        {

            ICustomerTODDataManager dataManager = BEDataManagerFactory.GetDataManager<ICustomerTODDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetCustomerToDConsiderationByCriteria(input));
        }
        public CustomerTODConsiderationInfo GetCustomerToDConsideration(string customerId, int zoneId, DateTime when)
        {
            TODConsiderationsByCustomer todConsiderationsByCustomer = _cacheManager.GetOrCreateObject(String.Format("GetEffectiveCustomerTODConsideration_{0}_{1:ddMMMyy}", customerId, when.Date),
            CacheObjectType.Pricing,
            () =>
            {
                return GetCustomerToDConsiderationsFromDB(when);
            });

            CustomerToDConsiderationByZone toDConsiderationByZoneObject;
            if (todConsiderationsByCustomer.TryGetValue(customerId, out toDConsiderationByZoneObject))
            {
                CustomerTODConsiderationInfo todConsiderationInfo;
                toDConsiderationByZoneObject.TryGetValue(zoneId, out todConsiderationInfo);
                return todConsiderationInfo;
            }
            return null;
        }
        private static TODConsiderationsByCustomer GetCustomerToDConsiderationsFromDB(DateTime when)
        {
            ICustomerTODDataManager dataManager = BEDataManagerFactory.GetDataManager<ICustomerTODDataManager>();
            List<CustomerTODConsiderationInfo> customersTODConsiderationInfo = dataManager.GetCustomersToDConsideration(when);
            TODConsiderationsByCustomer todConsiderationsByCustomer = new TODConsiderationsByCustomer();
            foreach (CustomerTODConsiderationInfo todConsiderationInfo in customersTODConsiderationInfo)
            {
                CustomerToDConsiderationByZone toDConsiderationByZoneObject;
                if (!todConsiderationsByCustomer.TryGetValue(todConsiderationInfo.CustomerID, out toDConsiderationByZoneObject))
                {
                    toDConsiderationByZoneObject = new CustomerToDConsiderationByZone();
                    todConsiderationsByCustomer.Add(todConsiderationInfo.CustomerID, toDConsiderationByZoneObject);
                }
                if (!toDConsiderationByZoneObject.ContainsKey(todConsiderationInfo.ZoneID))
                    toDConsiderationByZoneObject.Add(todConsiderationInfo.ZoneID, todConsiderationInfo);
            }
            return todConsiderationsByCustomer;
        }
    }
}

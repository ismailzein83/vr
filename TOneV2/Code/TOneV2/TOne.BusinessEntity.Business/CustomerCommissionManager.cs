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
    public class CustomerCommissionManager
    {
         TOneCacheManager _cacheManager;
         public CustomerCommissionManager()
         {
         }
         public CustomerCommissionManager(TOneCacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }
         public Vanrise.Entities.IDataRetrievalResult<CustomerCommission> GetCustomerCommissions(Vanrise.Entities.DataRetrievalInput<CustomerCommissionQuery> input)
        {
            ICustomerCommissionDataManager dataManager = BEDataManagerFactory.GetDataManager<ICustomerCommissionDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetCustomerCommissions(input));
        }
       public CustomerCommission GetCustomerCommissions(string customerId,int zoneId, DateTime when)
        {
            CommissionsByCustomer commissionsByCustomer = _cacheManager.GetOrCreateObject(String.Format("GetEffectiveCustomerCommission_{0}_{1:ddMMMyy}", customerId, when.Date),
              CacheObjectType.Pricing,
              () =>
              {
                  return GetCustomerCommissionsFromDB(when);
              });

            CustomerCommissionsByZone commissionsByZoneObject;
            if (commissionsByCustomer.TryGetValue(customerId, out commissionsByZoneObject))
            {
                CustomerCommission commission;
                commissionsByZoneObject.TryGetValue(zoneId, out commission);
                return commission;
            }
            return null;
        }

       private CommissionsByCustomer GetCustomerCommissionsFromDB(DateTime when)
       {
           ICustomerCommissionDataManager dataManager = BEDataManagerFactory.GetDataManager<ICustomerCommissionDataManager>();
           List<CustomerCommission> customerCommissions = dataManager.GetCustomerCommissions(when);
           CommissionsByCustomer commissionsByCustomer = new CommissionsByCustomer();
           foreach (CustomerCommission commission in customerCommissions)
           {
               CustomerCommissionsByZone customerCommissionsByZone;
               if (!commissionsByCustomer.TryGetValue(commission.CustomerId, out customerCommissionsByZone))
               {
                   customerCommissionsByZone = new CustomerCommissionsByZone();
                   commissionsByCustomer.Add(commission.CustomerId, customerCommissionsByZone);
               }
               if (!customerCommissionsByZone.ContainsKey(commission.ZoneId))
                   customerCommissionsByZone.Add(commission.ZoneId, commission);
           }
           return commissionsByCustomer;
       }
    }
}

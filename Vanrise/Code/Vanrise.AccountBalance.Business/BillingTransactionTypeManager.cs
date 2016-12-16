using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Data;
using Vanrise.AccountBalance.Entities;
using Vanrise.Common;
namespace Vanrise.AccountBalance.Business
{
    public class BillingTransactionTypeManager
    {

        public BillingTransactionType GetBillingTransactionType(Guid billingTransactionTypeId)
        {
            Dictionary<Guid, BillingTransactionType> cachedBillingTransactionTypes = this.GetCachedBillingTransactionTypes();
            return cachedBillingTransactionTypes.GetRecord(billingTransactionTypeId);
        }

        public string GetBillingTransactionTypeName(Guid billingTransactionTypeId)
        {
            BillingTransactionType billingTransactionType = this.GetBillingTransactionType(billingTransactionTypeId);
            return (billingTransactionType != null) ? billingTransactionType.Name : null;
        }

        public bool IsCredit(Guid billingTransactionTypeId)
        {
            BillingTransactionType billingTransactionType = this.GetBillingTransactionType(billingTransactionTypeId);
            return billingTransactionType.IsCredit;
        }

        #region Private Classes
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IBillingTransactionTypeDataManager _dataManager = AccountBalanceDataManagerFactory.GetDataManager<IBillingTransactionTypeDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreBillingTransactionTypeUpdated(ref _updateHandle);
            }
        }
        #endregion

        #region Private Methods

        Dictionary<Guid, BillingTransactionType> GetCachedBillingTransactionTypes()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedBillingTransactionTypes",
               () =>
               {
                   IBillingTransactionTypeDataManager dataManager = AccountBalanceDataManagerFactory.GetDataManager<IBillingTransactionTypeDataManager>();
                   return dataManager.GetBillingTransactionTypes().ToDictionary(x => x.BillingTransactionTypeId, x => x);
               });
        }

        #endregion

        public object GetBillingTransactionTypesInfo()
        {
            return GetCachedBillingTransactionTypes().MapRecords(BillingTransactionTypeInfoMapper);
        }

        BillingTransactionTypeInfo BillingTransactionTypeInfoMapper(BillingTransactionType transactionType)
        {
            return new BillingTransactionTypeInfo
            {
                Id = transactionType.BillingTransactionTypeId,
                Name = transactionType.Name
            };
        }
    }
}

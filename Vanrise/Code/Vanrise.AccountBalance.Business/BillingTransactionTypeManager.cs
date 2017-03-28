using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.AccountBalance.Data;
using Vanrise.AccountBalance.Entities;
using Vanrise.Common;

namespace Vanrise.AccountBalance.Business
{
    public class BillingTransactionTypeManager
    {
        #region Public Methods

        public IEnumerable<BillingTransactionType> GetBillingTransactionTypes()
        {
            return this.GetCachedBillingTransactionTypes().Values;
        }

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

        public IEnumerable<BillingTransactionTypeInfo> GetBillingTransactionTypesInfo(BillingTransactionTypeInfoFilter filter)
        {
            Dictionary<Guid, BillingTransactionType> cachedBillingTransactionTypes = GetCachedBillingTransactionTypes();

            Func<BillingTransactionType, bool> filterExpression = null;

            if (filter != null)
            {
                IEnumerable<Guid> allowedBillingTransactionTypeIds = null;

                if (filter.AccountTypeId.HasValue)
                {
                    allowedBillingTransactionTypeIds = new AccountTypeManager().GetAllowedBillingTransactionTypeIds(filter.AccountTypeId.Value);

                    if (allowedBillingTransactionTypeIds == null || allowedBillingTransactionTypeIds.Count() == 0)
                        throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("AllowedBillingTransactionTypes of AccountType '{0}' where not found", filter.AccountTypeId.Value));
                }

                filterExpression = (billingTransactionType) =>
                {
                    if (allowedBillingTransactionTypeIds != null && !allowedBillingTransactionTypeIds.Contains(billingTransactionType.BillingTransactionTypeId))
                        return false;

                    if (filter.Filters != null && filter.Filters.Any(x => !x.IsMatched(billingTransactionType.Settings)))
                        return false;

                    return true;
                };
            }

            return cachedBillingTransactionTypes.MapRecords(BillingTransactionTypeInfoMapper, filterExpression);
        }

        #endregion

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

        private Dictionary<Guid, BillingTransactionType> GetCachedBillingTransactionTypes()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedBillingTransactionTypes",
               () =>
               {
                   IBillingTransactionTypeDataManager dataManager = AccountBalanceDataManagerFactory.GetDataManager<IBillingTransactionTypeDataManager>();
                   return dataManager.GetBillingTransactionTypes().ToDictionary(x => x.BillingTransactionTypeId, x => x);
               });
        }

        #endregion

        #region Mappers

        BillingTransactionTypeInfo BillingTransactionTypeInfoMapper(BillingTransactionType transactionType)
        {
            return new BillingTransactionTypeInfo
            {
                Id = transactionType.BillingTransactionTypeId,
                Name = transactionType.Name
            };
        }

        #endregion
    }
}

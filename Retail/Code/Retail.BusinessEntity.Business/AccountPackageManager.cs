using Retail.BusinessEntity.Data;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.Common;

namespace Retail.BusinessEntity.Business
{
    public class AccountPackageManager
    {
        #region Fields

        AccountManager _accountManager = new AccountManager();
        PackageManager _packageManager = new PackageManager();

        #endregion
        
        #region Public Methods

        public Vanrise.Entities.IDataRetrievalResult<AccountPackageDetail> GetFilteredAccountPackages(Vanrise.Entities.DataRetrievalInput<AccountPackageQuery> input)
        {
            Dictionary<int, AccountPackage> cachedAccountPackages = this.GetCachedAccountPackages();
            Func<AccountPackage, bool> filterExpression = (accountPackage) => (accountPackage.AccountId == input.Query.AssignedToAccountId);
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, cachedAccountPackages.ToBigResult(input, filterExpression, AccountPackageDetailMapper));
        }

        public AccountPackage GetAccountPackage(int accountPackageId)
        {
            Dictionary<int, AccountPackage> cachedAccountPackages = this.GetCachedAccountPackages();
            return cachedAccountPackages.GetRecord(accountPackageId);
        }

        public Vanrise.Entities.InsertOperationOutput<AccountPackageDetail> AddAccountPackage(AccountPackage accountPackage)
        {
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<AccountPackageDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            IAccountPackageDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountPackageDataManager>();
            int accountPackageId = -1;

            if (dataManager.Insert(accountPackage, out accountPackageId))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                accountPackage.AccountPackageId = accountPackageId;
                insertOperationOutput.InsertedObject = this.AccountPackageDetailMapper(accountPackage);
            }

            return insertOperationOutput;
        }

        #endregion

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IAccountPackageDataManager _dataManager = BEDataManagerFactory.GetDataManager<IAccountPackageDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreAccountPackagesUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region Private Methods

        Dictionary<int, AccountPackage> GetCachedAccountPackages()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetAccountPackages", () =>
            {
                IAccountPackageDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountPackageDataManager>();
                IEnumerable<AccountPackage> accountPackages = dataManager.GetAccountPackages();
                return accountPackages.ToDictionary(kvp => kvp.AccountPackageId, kvp => kvp);
            });
        }

        #endregion

        #region Mappers

        AccountPackageDetail AccountPackageDetailMapper(AccountPackage accountPackage)
        {
            return new AccountPackageDetail()
            {
                Entity = accountPackage,
                AccountName = _accountManager.GetAccountName(accountPackage.AccountId),
                PackageName = _packageManager.GetPackageName(accountPackage.PackageId)
            };
        }
        
        #endregion
    }
}

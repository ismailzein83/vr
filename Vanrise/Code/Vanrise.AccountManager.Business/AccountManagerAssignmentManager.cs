using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountManager.Data;
using Vanrise.AccountManager.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Security.Business;

namespace Vanrise.AccountManager.Business
{
    public class AccountManagerAssignmentManager
    {
        public AccountManagerAssignment GetAccountManagerAssignment(long accountManagerAssignmentId)
        {
            var allAccountManagerAssignments = this.GetCachedAccountManagerAssignments();
            return allAccountManagerAssignments.GetRecord(accountManagerAssignmentId);
        }
        public IEnumerable<AccountManagerAssignment> GetAccountManagerAssignments()
        {
             return this.GetCachedAccountManagerAssignments().Values;
        }
        public bool TryAddAccountManagerAssignment(AccountManagerAssignment accountManagerAssignment, out int insertedID, out string errorMessage)
        {
            insertedID = -1;
            errorMessage = null;
            IAccountManagerAssignmentDataManager dataManager = AccountManagerDataManagerFactory.GetDataManager<IAccountManagerAssignmentDataManager>();
            bool isAdded = dataManager.AddAccountManagerAssignment(accountManagerAssignment, out insertedID);
            if (isAdded)
            {
                accountManagerAssignment.AccountManagerAssignementId = insertedID;
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
            }
            return isAdded;
        }
        public bool TryUpdateAccountManagerAssignment(AccountManagerAssignment accountManagerAssignment, out string errorMessage)
        {
            errorMessage = null;
            IAccountManagerAssignmentDataManager dataManager = AccountManagerDataManagerFactory.GetDataManager<IAccountManagerAssignmentDataManager>();
            bool isUpdated = dataManager.UpdateAccountManagerAssignment(accountManagerAssignment);
            if (isUpdated)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
            }
            return isUpdated;
        }
        public void AssignAccountManagerToAccounts(AssignAccountManagerToAccountsInput input)
        {
            throw new NotImplementedException();
        }
        public void UpdateAccountManagerAssignment(UpdateAccountManagerAssignmentInput input)
        {
            throw new NotImplementedException();
        }

        public bool IsAccountAssignableToAccountManager(Guid assignmentDefinitionId, string accountId)
        {
            throw new NotImplementedException();
        }

        public AccountManagerAssignment GetAccountAssignment(Guid assignmentDefinitionId, string accountId, DateTime effectiveOn)
        {
            throw new NotImplementedException();
        }
        #region Private Classes
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IAccountManagerAssignmentDataManager dataManager = AccountManagerDataManagerFactory.GetDataManager<IAccountManagerAssignmentDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return dataManager.AreAccountManagerAssignmentsUpdated(ref _updateHandle);
            }
        }
        #endregion

        #region Private Methods
        Dictionary<long,AccountManagerAssignment> GetCachedAccountManagerAssignments()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetAccountManagerAssignments",
               () =>
               {
                   IAccountManagerAssignmentDataManager dataManager = AccountManagerDataManagerFactory.GetDataManager<IAccountManagerAssignmentDataManager>();
                   IEnumerable<Vanrise.AccountManager.Entities.AccountManagerAssignment> accountManagerAssignments = dataManager.GetAccountManagerAssignments();
                   return accountManagerAssignments.ToDictionary(cn => cn.AccountManagerAssignementId, cn => cn);
               });
        }
        #endregion
        #region Mappers

        #endregion
    }

    public class AssignAccountManagerToAccountsInput
    {
        public Guid AccountManagerAssignementDefinitionId { get; set; }

        public long AccountManagerId { get; set; }

        public List<AssignAccountManagerToAccountSetting> Accounts { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }

        ///// <summary>
        ///// later we can implement, for now stop if anyone is invalid
        ///// </summary>
        //public bool ContinueIfInvalid { get; set; }
    }

    public class AssignAccountManagerToAccountSetting
    {
        public string AccountId { get; set; }

        public AccountManagerAssignmentSettings AssignementSettings { get; set; }
    }

    public class AssignAccountManagerToAccountsOutput
    {

    }

    public class UpdateAccountManagerAssignmentInput
    {
        public long AccountManagerAssignmentId { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }

        public AccountManagerAssignmentSettings AssignementSettings { get; set; }
    }
     

}

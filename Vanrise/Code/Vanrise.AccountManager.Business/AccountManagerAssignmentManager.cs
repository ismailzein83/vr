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
        #region Public Methods
        public AccountManagerAssignment GetAccountManagerAssignment(long accountManagerAssignmentId)
        {
            var allAccountManagerAssignments = this.GetCachedAccountManagerAssignments();
            return allAccountManagerAssignments.GetRecord(accountManagerAssignmentId);
        }
        public IEnumerable<AccountManagerAssignment> GetAccountManagerAssignments()
        {
            return this.GetCachedAccountManagerAssignments().Values;
        }
        internal bool TryAddAccountManagerAssignment(AccountManagerAssignment accountManagerAssignment, out int insertedID, out string errorMessage)
        {
            insertedID = -1;
            errorMessage = null;
            IAccountManagerAssignmentDataManager dataManager = AccountManagerDataManagerFactory.GetDataManager<IAccountManagerAssignmentDataManager>();
            bool insertActionSucc = dataManager.AddAccountManagerAssignment(accountManagerAssignment, out insertedID);
            if (insertActionSucc)
            {
                accountManagerAssignment.AccountManagerAssignementId = insertedID;
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
            }
            return insertActionSucc;
        }
        internal bool TryUpdateAccountManagerAssignment(long accountManagerAssignmentId, DateTime bed, DateTime? eed, AccountManagerAssignmentSettings settings)
        {
            IAccountManagerAssignmentDataManager dataManager = AccountManagerDataManagerFactory.GetDataManager<IAccountManagerAssignmentDataManager>();
            bool updateActionSucc = dataManager.UpdateAccountManagerAssignment(accountManagerAssignmentId, bed, eed, settings);
            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
            }
            return updateActionSucc;
        }
        public bool AssignAccountManagerToAccounts(AssignAccountManagerToAccountsInput input, out string errorMessage)
        {
            AccountManagerAssignment accountManagerAssignment = new AccountManagerAssignment();
            accountManagerAssignment.AccountManagerAssignementDefinitionId = input.AccountManagerAssignementDefinitionId;
            accountManagerAssignment.AccountManagerId = input.AccountManagerId;
            accountManagerAssignment.BED = input.BED;
            accountManagerAssignment.EED = input.EED;
            int insertedID;
            errorMessage = null;
            bool insertActionSucc = false;
            string errorString;
            if (input.Accounts != null)
            {
                foreach (var account in input.Accounts)
                {
                    accountManagerAssignment.AccountId = account.AccountId;
                    accountManagerAssignment.Settings = account.AssignementSettings;
                    insertActionSucc = TryAddAccountManagerAssignment(accountManagerAssignment, out insertedID, out errorString);
                }
            }
            return insertActionSucc;
        }
        public bool UpdateAccountManagerAssignment(UpdateAccountManagerAssignmentInput input, out string errorMessage)
        {
            bool updateActionSucc = false;
            errorMessage = null;
            if (input != null)
            {
                updateActionSucc = TryUpdateAccountManagerAssignment(input.AccountManagerAssignmentId, input.BED, input.EED, input.AssignementSettings);
            }
            return updateActionSucc;
        }

        public bool IsAccountAssignableToAccountManager(Guid assignmentDefinitionId, string accountId)
        {
            throw new NotImplementedException();
        }

        public AccountManagerAssignment GetAccountAssignment(Guid assignmentDefinitionId, string accountId, DateTime effectiveOn)
        {
            throw new NotImplementedException();
        }
        #endregion

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
        Dictionary<long, AccountManagerAssignment> GetCachedAccountManagerAssignments()
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
    #region Public Classes
    
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
     #endregion

}

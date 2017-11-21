using System;
using System.Collections.Concurrent;
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
        public AccountManagerAssignment GetAccountManagerAssignment(long accountManagerAssignmentId,Guid accountManagerAssignmentDefinitionId)
        {
            var allAccountManagerAssignments = this.GetCachedAccountManagerAssignments(accountManagerAssignmentDefinitionId);
            return allAccountManagerAssignments.GetRecord(accountManagerAssignmentId);
        }
        public IEnumerable<AccountManagerAssignment> GetAccountManagerAssignments(Guid accountManagerAssignmentDefinitionId)
        {
            return this.GetCachedAccountManagerAssignments(accountManagerAssignmentDefinitionId).Values;
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
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired(accountManagerAssignment.AccountManagerAssignementDefinitionId);
            }
            return insertActionSucc;
        }
        internal bool TryUpdateAccountManagerAssignment(long accountManagerAssignmentId, DateTime bed, DateTime? eed, AccountManagerAssignmentSettings settings, Guid accountManagerAssignementDefinitionId)
        {
            IAccountManagerAssignmentDataManager dataManager = AccountManagerDataManagerFactory.GetDataManager<IAccountManagerAssignmentDataManager>();
            bool updateActionSucc = dataManager.UpdateAccountManagerAssignment(accountManagerAssignmentId, bed, eed, settings);
            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired(accountManagerAssignementDefinitionId);
            }
            return updateActionSucc;
        }
        public bool AssignAccountManagerToAccounts(AssignAccountManagerToAccountsInput input, out string insertErrorMessage)
        {
            insertErrorMessage = null;
            int insertedID;
            string errorMessage = null;
            bool insertActionSucc = false;
            string errorString;
            if (input.Accounts != null)
            {
                if (!ValidateAccountManagerAssignmentInputs(input.Accounts, input.AccountManagerAssignementDefinitionId, input.BED, input.EED, out errorMessage))
                {
                    foreach (var account in input.Accounts)
                    {
                        AccountManagerAssignment accountManagerAssignment = new AccountManagerAssignment()
                        {
                            AccountManagerAssignementDefinitionId = input.AccountManagerAssignementDefinitionId,
                            AccountManagerId = input.AccountManagerId,
                            BED = input.BED,
                            EED = input.EED
                        };
                        accountManagerAssignment.AccountId = account.AccountId;
                        accountManagerAssignment.Settings = account.AssignementSettings;
                        insertActionSucc = TryAddAccountManagerAssignment(accountManagerAssignment, out insertedID, out errorString);
                    }
                }
                else
                {
                    insertErrorMessage = errorMessage;
                }
            }
            return insertActionSucc;
        }
        public bool UpdateAccountManagerAssignment(UpdateAccountManagerAssignmentInput input, out string updateErrorMessage)
        {
            string errorMessage;
            var accountManagerAssignment = GetAccountManagerAssignment(input.AccountManagerAssignmentId,input.AccountManagerAssignmentDefinitionId);
            bool updateActionSucc = false;
            updateErrorMessage = null;
            if (input != null)
            {
                if (!ValidateAccountManagerAssignmentInput(accountManagerAssignment.AccountId, accountManagerAssignment.AccountManagerAssignementDefinitionId, input.BED, input.EED,input.AccountManagerAssignmentId, out errorMessage))
                {
                    updateActionSucc = TryUpdateAccountManagerAssignment(input.AccountManagerAssignmentId, input.BED, input.EED, input.AssignementSettings, input.AccountManagerAssignmentDefinitionId);
                }
                else
                {
                    updateErrorMessage = errorMessage;
                }
            }
            return updateActionSucc;
        }
        public bool ValidateAccountManagerAssignmentInputs(List<AssignAccountManagerToAccountSetting> accounts, Guid accountManagerAssignementDefinitionId, DateTime bed, DateTime? eed, out string errorMessage)
        {
            
            errorMessage = null;
            List<string> overLappedAccountNames = new List<string>();
            string overLappedAccountName;
            bool areOverLapped = false;
            if (accounts != null)
            {
                if (!ValidateDateTime(bed,eed))
                {
                    errorMessage = "BED cannot be greater than EED";
                    return true;
                }

                foreach (var account in accounts)
                {
                    if (IsAccountOverLapped(account.AccountId, accountManagerAssignementDefinitionId, bed, eed,null, out overLappedAccountName))
                    {
                        overLappedAccountNames.Add(overLappedAccountName);
                        areOverLapped = true;
                    }
                }
                if (areOverLapped == true)
                {
                    errorMessage = string.Format("Specified Interval overlaps with other assignments of Account(s): {0}", string.Join(", ", overLappedAccountNames));
                }
            }
            return areOverLapped;
        }
        public bool AreAccountAssignableToAccountManager(Guid assignmentDefinitionId, string accountId)
        {
            throw new NotImplementedException();
        }
        public bool IsAccountOverLapped(string accountId, Guid accountManagerAssignementDefinitionId, DateTime bed, DateTime? eed,long? accountManagerAssignmentId, out string overLappedAccountName)
        {
            AccountManagerDefinitionManager accountManagerDefinitionManager = new AccountManagerDefinitionManager();
            AccountManagerManager accountManagerManager = new AccountManagerManager();
            bool isOverlapped = false;
            overLappedAccountName = null;
            IEnumerable<AccountManagerAssignment> accountManagerAssignments = new List<AccountManagerAssignment>();
            accountManagerAssignments = GetAccountManagerAssignmentsById(accountId, accountManagerAssignementDefinitionId);
            foreach (var accountManagerAssignment in accountManagerAssignments)
            {
                if (Utilities.AreTimePeriodsOverlapped(bed, eed, accountManagerAssignment.BED, accountManagerAssignment.EED) && accountManagerAssignmentId != accountManagerAssignment.AccountManagerAssignementId)
                {
                    var accountManagerDefinitionId = accountManagerManager.GetAccountManager(accountManagerAssignment.AccountManagerId).AccountManagerDefinitionId;
                    var accountManagerAssignmentDefinition = accountManagerDefinitionManager.GetAccountManagerAssignmentDefinition(accountManagerDefinitionId, accountManagerAssignementDefinitionId);
                    overLappedAccountName = accountManagerAssignmentDefinition.Settings.GetAccountName(accountManagerAssignment.AccountId);
                    return true;
                }
            }
            return isOverlapped;
        }
        public bool ValidateAccountManagerAssignmentInput(string accountId, Guid accountManagerAssignementDefinitionId, DateTime bed, DateTime? eed,long accountManagerAssignmentId, out string errorMessage)
        {
            string overLappedAccountName;
            errorMessage = null;
            if (!ValidateDateTime(bed, eed))
            {
                errorMessage = "BED cannot be greater than EED";
                return true;
            }
            bool isOverLapped = IsAccountOverLapped(accountId, accountManagerAssignementDefinitionId, bed, eed, accountManagerAssignmentId, out overLappedAccountName);
            if (isOverLapped)
                errorMessage = string.Format("Specified Interval overlaps with other assignments of Account(s): {0}", overLappedAccountName);
            return isOverLapped;
        }
        public IEnumerable<AccountManagerAssignment> GetAccountManagerAssignmentsById(string accountId, Guid accountManagerAssignementDefinitionId)
        {
            var allAccountMaanagerAssignments = this.GetCachedAccountManagerAssignments(accountManagerAssignementDefinitionId).Values;
            return allAccountMaanagerAssignments.FindAllRecords(item => item.AccountId == accountId && item.AccountManagerAssignementDefinitionId == accountManagerAssignementDefinitionId);
        }
        public AccountManagerAssignment GetAccountAssignment(Guid assignmentDefinitionId, string accountId, DateTime effectiveOn)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Private Classes
        private class CacheManager : Vanrise.Caching.BaseCacheManager<Guid>
        {
            IAccountManagerAssignmentDataManager dataManager = AccountManagerDataManagerFactory.GetDataManager<IAccountManagerAssignmentDataManager>();
           
            ConcurrentDictionary<Guid, Object> _updateHandlesByAsignmentDefinitionId = new ConcurrentDictionary<Guid, Object>();
            protected override bool ShouldSetCacheExpired(Guid accountManagerAssignmentDefinitionId)
            {
                object _updateHandle;
                _updateHandlesByAsignmentDefinitionId.TryGetValue(accountManagerAssignmentDefinitionId, out _updateHandle);
                bool isCacheExpired = dataManager.AreAccountManagerAssignmentsUpdated(accountManagerAssignmentDefinitionId, ref _updateHandle);
                _updateHandlesByAsignmentDefinitionId.AddOrUpdate(accountManagerAssignmentDefinitionId, _updateHandle, (key, existingHandle) => _updateHandle);
                return isCacheExpired;
            }
        }
        #endregion

        #region Private Methods
        Dictionary<long, AccountManagerAssignment> GetCachedAccountManagerAssignments(Guid accountManagerAssignmentDefinitionId)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>(accountManagerAssignmentDefinitionId).GetOrCreateObject("GetAccountManagerAssignments", accountManagerAssignmentDefinitionId,
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

        #region Private Methods

        private bool ValidateDateTime(DateTime bed, DateTime? eed)
        {
            if (bed > eed)
                return false;
            return true;
        }
        
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
        public Guid AccountManagerAssignmentDefinitionId { get; set; }
        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }

        public AccountManagerAssignmentSettings AssignementSettings { get; set; }
    }
     #endregion

}

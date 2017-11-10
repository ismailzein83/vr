﻿using System;
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
            List<string> overLappedAccountNames = new List<string>();
            int insertedID;
            errorMessage = null;
            bool insertActionSucc = false;
            string errorString;
            if (input.Accounts != null)
            {
                if (!AreAccountsOverLapped(input.Accounts, input.AccountManagerAssignementDefinitionId, input.BED, input.EED, out overLappedAccountNames))
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
                    errorMessage = string.Format("Specified Interval overlaps with other assignments of Account(s): {0}", string.Join(", ", overLappedAccountNames));
                }
            }
            return insertActionSucc;
        }
        public bool UpdateAccountManagerAssignment(UpdateAccountManagerAssignmentInput input, out string errorMessage)
        {
            string overLappedAccountName;
            var accountManagerAssignment = GetAccountManagerAssignment(input.AccountManagerAssignmentId);
            bool updateActionSucc = false;
            errorMessage = null;
            if (input != null)
            {
                if (!IsAccountOverLapped(accountManagerAssignment.AccountId, accountManagerAssignment.AccountManagerAssignementDefinitionId, input.BED, input.EED, out overLappedAccountName))
                {
                    updateActionSucc = TryUpdateAccountManagerAssignment(input.AccountManagerAssignmentId, input.BED, input.EED, input.AssignementSettings);
                }
                else
                {
                    errorMessage = string.Format("Specified Interval overlaps with other assignments of Account: {0}", overLappedAccountName);
                }
            }
            return updateActionSucc;
        }
        public bool AreAccountsOverLapped(List<AssignAccountManagerToAccountSetting> accounts, Guid accountManagerAssignementDefinitionId, DateTime bed, DateTime? eed, out List<string> overLappedAccountNames)
        {
            overLappedAccountNames = new List<string>();
            string overLappedAccountName;
            bool areOverLapped = false;
            if (accounts != null)
            {
                foreach (var account in accounts)
                {
                    if (IsAccountOverLapped(account.AccountId, accountManagerAssignementDefinitionId, bed, eed, out overLappedAccountName))
                    {
                        overLappedAccountNames.Add(overLappedAccountName);
                        areOverLapped = true;
                    }

                }
            }
            return areOverLapped;
        }
        public bool AreAccountAssignableToAccountManager(Guid assignmentDefinitionId, string accountId)
        {
            throw new NotImplementedException();
        }
        public bool IsAccountOverLapped(string accountId, Guid accountManagerAssignementDefinitionId, DateTime bed, DateTime? eed, out string overLappedAccountName)
        {
            AccountManagerDefinitionManager accountManagerDefinitionManager = new AccountManagerDefinitionManager();
            AccountManagerManager accountManagerManager = new AccountManagerManager();
            bool isOverlapped = false;
            overLappedAccountName = null;
            IEnumerable<AccountManagerAssignment> accountManagerAssignments = new List<AccountManagerAssignment>();
            accountManagerAssignments = GetAccountManagerAssignmentsById(accountId, accountManagerAssignementDefinitionId);
            foreach (var accountManagerAssignment in accountManagerAssignments)
            {
                if (Utilities.AreTimePeriodsOverlapped(bed, eed, accountManagerAssignment.BED, accountManagerAssignment.EED))
                {
                    var accountManagerDefinitionId = accountManagerManager.GetAccountManager(accountManagerAssignment.AccountManagerId).AccountManagerDefinitionId;
                    var accountManagerAssignmentDefinition = accountManagerDefinitionManager.GetAccountManagerAssignmentDefinition(accountManagerDefinitionId, accountManagerAssignementDefinitionId);
                    overLappedAccountName = accountManagerAssignmentDefinition.Settings.GetAccountName(accountManagerAssignment.AccountId);
                    return true;
                }
            }
            return isOverlapped;
        }
        public IEnumerable<AccountManagerAssignment> GetAccountManagerAssignmentsById(string accountId, Guid accountManagerAssignementDefinitionId)
        {
            var allAccountMaanagerAssignments = this.GetCachedAccountManagerAssignments().Values;
            return allAccountMaanagerAssignments.FindAllRecords(item => item.AccountId == accountId && item.AccountManagerAssignementDefinitionId == accountManagerAssignementDefinitionId);
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

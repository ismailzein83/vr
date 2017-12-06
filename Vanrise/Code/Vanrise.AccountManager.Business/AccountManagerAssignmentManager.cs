﻿using System;
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
        public AccountManagerAssignment GetAccountManagerAssignment(long accountManagerAssignmentId, Guid accountManagerAssignmentDefinitionId)
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
            AccountManagerDefinitionManager accountManagerDefinitionManager = new AccountManagerDefinitionManager();
            insertedID = -1;
            errorMessage = null;
            IAccountManagerAssignmentDataManager dataManager = AccountManagerDataManagerFactory.GetDataManager<IAccountManagerAssignmentDataManager>();
            bool insertActionSucc = dataManager.AddAccountManagerAssignment(accountManagerAssignment, out insertedID);
            if (insertActionSucc)
            {
                var assignmentDefinition = accountManagerDefinitionManager.GetAccountManagerAssignmnetInfoByAssignmentDefinitionId(accountManagerAssignment.AccountManagerAssignementDefinitionId).AssignmentDefinition;
                AssignmentDefinitionTrackAndLogObject assignmentDefinitionTrackAndLogObject = new AssignmentDefinitionTrackAndLogObject();
                assignmentDefinitionTrackAndLogObject.AccountManagerAssignment = accountManagerAssignment;
                accountManagerAssignment.AccountManagerAssignementId = insertedID;
                VRActionLogger.Current.TrackAndLogObjectAdded(new AccountManagerAssignmnetLoggableEntity(accountManagerAssignment.AccountManagerAssignementDefinitionId), accountManagerAssignment);
                assignmentDefinition.Settings.TrackAndLogObject(assignmentDefinitionTrackAndLogObject);
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired(accountManagerAssignment.AccountManagerAssignementDefinitionId);
            }
            return insertActionSucc;
        }
        internal bool TryUpdateAccountManagerAssignment(long accountManagerAssignmentId, DateTime bed, DateTime? eed, AccountManagerAssignmentSettings settings, Guid accountManagerAssignementDefinitionId)
        {
            AccountManagerDefinitionManager accountManagerDefinitionManager = new AccountManagerDefinitionManager();
            IAccountManagerAssignmentDataManager dataManager = AccountManagerDataManagerFactory.GetDataManager<IAccountManagerAssignmentDataManager>();
            bool updateActionSucc = dataManager.UpdateAccountManagerAssignment(accountManagerAssignmentId, bed, eed, settings);
            if (updateActionSucc)
            {
                var accountManagerAssignment = GetAccountManagerAssignment(accountManagerAssignmentId, accountManagerAssignementDefinitionId);
                var assignmentDefinition = accountManagerDefinitionManager.GetAccountManagerAssignmnetInfoByAssignmentDefinitionId(accountManagerAssignementDefinitionId).AssignmentDefinition;
                AssignmentDefinitionTrackAndLogObject assignmentDefinitionTrackAndLogObject = new AssignmentDefinitionTrackAndLogObject();
                assignmentDefinitionTrackAndLogObject.AccountManagerAssignment = accountManagerAssignment;
                VRActionLogger.Current.TrackAndLogObjectUpdated(new AccountManagerAssignmnetLoggableEntity(accountManagerAssignementDefinitionId), accountManagerAssignment);
                assignmentDefinition.Settings.TrackAndLogObject(assignmentDefinitionTrackAndLogObject);
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
            var accountManagerAssignment = GetAccountManagerAssignment(input.AccountManagerAssignmentId, input.AccountManagerAssignmentDefinitionId);
            bool updateActionSucc = false;
            updateErrorMessage = null;
            if (input != null)
            {
                if (!ValidateAccountManagerAssignmentInput(accountManagerAssignment.AccountId, accountManagerAssignment.AccountManagerAssignementDefinitionId, input.BED, input.EED, input.AccountManagerAssignmentId, out errorMessage))
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
                if (!ValidateDateTime(bed, eed))
                {
                    errorMessage = "BED cannot be greater than EED";
                    return true;
                }

                foreach (var account in accounts)
                {
                    if (IsAccountOverLapped(account.AccountId, accountManagerAssignementDefinitionId, bed, eed, null, out overLappedAccountName))
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

            var accountManagerAssignmnents = this.GetCachedAccountManagerAssignments(assignmentDefinitionId);
            List<AccountManagerAssignment> accountAssignments = new List<AccountManagerAssignment>();
            foreach (var accountManagerAssignmnet in accountManagerAssignmnents)
            {
                if (accountManagerAssignmnet.Value.AccountId == accountId)
                {
                    accountAssignments.Add(accountManagerAssignmnet.Value);
                }
            }
            return AreAccountsValid(accountAssignments);
        }
        private bool AreAccountsValid(List<AccountManagerAssignment> accountManagerAssignments)
        {
            DateTime thisDay = DateTime.Today;
            foreach (var accountManagerAssignmnet in accountManagerAssignments)
            {
                if (accountManagerAssignmnet.BED < thisDay && !accountManagerAssignmnet.EED.HasValue)
                {
                    return false;
                }
            }
            return true;
        }
        public bool IsAccountOverLapped(string accountId, Guid accountManagerAssignementDefinitionId, DateTime bed, DateTime? eed, long? accountManagerAssignmentId, out string overLappedAccountName)
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
        public bool ValidateAccountManagerAssignmentInput(string accountId, Guid accountManagerAssignementDefinitionId, DateTime bed, DateTime? eed, long accountManagerAssignmentId, out string errorMessage)
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
            return allAccountMaanagerAssignments.FindAllRecords(item => item.AccountId == accountId);
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
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetAccountManagerAssignments", accountManagerAssignmentDefinitionId,
               () =>
               {
                   IAccountManagerAssignmentDataManager dataManager = AccountManagerDataManagerFactory.GetDataManager<IAccountManagerAssignmentDataManager>();
                   IEnumerable<Vanrise.AccountManager.Entities.AccountManagerAssignment> accountManagerAssignments = dataManager.GetAccountManagerAssignments(accountManagerAssignmentDefinitionId);
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

        #region Security
        public bool HasViewAssignmentPermission(Guid accountManagerDefinitionId)
        {
            AccountManagerManager accountManager = new AccountManagerManager();
            int userId = SecurityContext.Current.GetLoggedInUserId();
            return accountManager.DoesUserHaveAccess(userId, accountManagerDefinitionId, (sec) => sec.ViewAssignmentRequiredPermission);
        }
        public bool HasManageAssignmentPermission(Guid accountManagerDefinitionId)
        {
            AccountManagerManager accountManager = new AccountManagerManager();
            int userId = SecurityContext.Current.GetLoggedInUserId();
            return accountManager.DoesUserHaveAccess(userId, accountManagerDefinitionId, (sec) => sec.ManageAssignmentRequiredPermission);

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
        public Guid AccountManagerDefinitionId { get; set; }

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
        public Guid AccountManagerDefinitionId { get; set; }

        public AccountManagerAssignmentSettings AssignementSettings { get; set; }
    }
     #endregion
    #region Private Classes

    public class AccountManagerAssignmnetLoggableEntity : VRLoggableEntityBase
    {
        AccountManagerAssignmentsInfo accountManageAssignmnetInfo;
        Guid _accountManagerAssignmentDefinitionId;
        static AccountManagerDefinitionManager s_accountManagerDefinitionManager = new AccountManagerDefinitionManager();
        static AccountManagerManager s_accountManagerManager = new AccountManagerManager();

        public AccountManagerAssignmnetLoggableEntity(Guid assignmentDefinitionId)
        {
            accountManageAssignmnetInfo = s_accountManagerDefinitionManager.GetAccountManagerAssignmnetInfoByAssignmentDefinitionId(assignmentDefinitionId);
            _accountManagerAssignmentDefinitionId = assignmentDefinitionId;
        }

        public override string EntityUniqueName
        {
            get { return String.Format("VR_AccountManager_AccountManagerAssignment_{0}", _accountManagerAssignmentDefinitionId); }
        }

        public override string EntityDisplayName
        {
            get { return accountManageAssignmnetInfo.AssignmentDefinition.Name; }
        }

        public override string ViewHistoryItemClientActionName
        {
            get { return "VR_AccountManager_AccountManagerAssignment_ViewHistoryItem"; }
        }


        public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
        {
            AccountManagerAssignment accountManagerAssignment = context.Object.CastWithValidate<AccountManagerAssignment>("context.Object");
            return accountManagerAssignment.AccountManagerAssignementId;
        }

        public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
        {
            AccountManagerAssignment accountManagerAssignment = context.Object.CastWithValidate<AccountManagerAssignment>("context.Object");
            if (accountManageAssignmnetInfo.AssignmentDefinition == null || accountManageAssignmnetInfo.AssignmentDefinition.Settings == null)
                throw new Exception("AssignmnetDefinition  is Null");
            return String.Format("{0}_To_{1}", s_accountManagerManager.GetAccountManagerName(accountManagerAssignment.AccountManagerId), accountManageAssignmnetInfo.AssignmentDefinition.Settings.GetAccountName(accountManagerAssignment.AccountId));
        }
        public override string ModuleName
        {
            get { return "Account Manager"; }
        }
    }
    #endregion
   
}

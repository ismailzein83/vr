using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using Retail.Teles.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
namespace Retail.Teles.Business
{
    public enum ConditionType { 
        CanChangeEnterpriseMapping = 0,
        AllowChangeUserRGs = 1, 
        AllowRevertUserRGs = 2, 
        AllowEnterpriseMap = 3, 
        AllowSiteMap = 4, 
        CanChangeSiteMapping = 5 ,
        AllowUserMap = 6, 
        CanChangeUserMapping = 7 
    }
    public class TelesAccountCondition : AccountCondition
    {
        public override Guid ConfigId { get { return new Guid("2C1CEA7E-96F1-4BB0-83DD-FE8BA4BA984C"); } }
        public ConditionType ConditionType { get; set; }
        public Guid CompanyTypeId { get; set; }
        public Guid SiteTypeId { get; set; }
        public Guid? UserTypeId { get; set; }
        public string ActionType { get; set; }

        static AccountBEManager _accountBEManager = new AccountBEManager();
        static AccountTypeManager _accountTypeManager = new AccountTypeManager();

        public override bool Evaluate(IAccountConditionEvaluationContext context)
        {
            switch(this.ConditionType)
            {
                case Business.ConditionType.CanChangeEnterpriseMapping:
                    return CanChangeCompanyMapping(context.Account, this.CompanyTypeId);
                case Business.ConditionType.AllowChangeUserRGs:
                    return AllowChangeUserRGs(context.Account, this.CompanyTypeId, this.SiteTypeId,this.UserTypeId, this.ActionType);
                case Business.ConditionType.AllowRevertUserRGs:
                    return AllowRevertUserRGs(context.Account, this.CompanyTypeId, this.SiteTypeId, this.UserTypeId, this.ActionType);
                case Business.ConditionType.AllowEnterpriseMap:
                    return AllowEnterpriseMap(context.Account, this.CompanyTypeId, this.SiteTypeId);
                case Business.ConditionType.AllowSiteMap:
                    return AllowSiteMap(context.Account, this.CompanyTypeId, this.SiteTypeId);
                case Business.ConditionType.CanChangeSiteMapping :
                    return CanChangeSiteMapping(context.Account, this.SiteTypeId);
                case Business.ConditionType.AllowUserMap:
                    return AllowUserMap(context.Account, this.UserTypeId);
                case Business.ConditionType.CanChangeUserMapping:
                    return CanChangeUserMapping(context.Account, this.UserTypeId);
                default:
                    return false;
            }
        }
        private static bool IsEnterpriseNotMappedOrProvisioned(Account account)
        {
            var enterpriseAccountMappingInfo = _accountBEManager.GetExtendedSettings<EnterpriseAccountMappingInfo>(account);
            return enterpriseAccountMappingInfo == null || enterpriseAccountMappingInfo.Status.HasValue;
        }
        private static bool IsSiteNotMappedOrProvisioned(Account account)
        {
            var siteAccountMappingInfo = _accountBEManager.GetExtendedSettings<SiteAccountMappingInfo>(account);
            return siteAccountMappingInfo == null || siteAccountMappingInfo.Status.HasValue;
        }
        private static bool IsEnterpriseMapped(Account account)
        {
            var enterpriseAccountMappingInfo = _accountBEManager.GetExtendedSettings<EnterpriseAccountMappingInfo>(account);
            return enterpriseAccountMappingInfo != null;
        }
        private static bool IsSiteMapped(Account account)
        {
            var siteAccountMappingInfo = _accountBEManager.GetExtendedSettings<SiteAccountMappingInfo>(account);
            return siteAccountMappingInfo != null;
        }
        private static bool IsUserMapped(Account account)
        {
            var userAccountMappingInfo = _accountBEManager.GetExtendedSettings<UserAccountMappingInfo>(account);
            return userAccountMappingInfo != null;
        }
        private static bool IsUserNotMappedOrProvisioned(Account account)
        {
            var userAccountMappingInfo = _accountBEManager.GetExtendedSettings<UserAccountMappingInfo>(account);
            return userAccountMappingInfo == null || userAccountMappingInfo.Status.HasValue;
        }

        private static bool IsChangedUserRGs(Account account, bool useActionType, string actionType)
        {
            var changeUsersRGsAccountState = _accountBEManager.GetExtendedSettings<ChangeUsersRGsAccountState>(account);
            if (changeUsersRGsAccountState != null)
            {
                if (useActionType)
                {
                    actionType.ThrowIfNull("ActionType");

                    if (changeUsersRGsAccountState.ChangesByActionType != null)
                    {
                        ChURGsActionCh chURGsActionCh;
                        if (changeUsersRGsAccountState.ChangesByActionType.TryGetValue(actionType, out chURGsActionCh))
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    return true;
                }

            }
            return false;
        }

        public static bool CanChangeCompanyMapping(Account account, Guid companyTypeId)
        {
            if (account.TypeId == companyTypeId)
            {
                if (IsEnterpriseNotMappedOrProvisioned(account))
                    return false;
                if (IsChangedUserRGs(account, false, null))
                    return false;

                var accountBEDefinitionId = _accountTypeManager.GetAccountBEDefinitionId(account.TypeId);
                var childAccounts = _accountBEManager.GetChildAccounts(accountBEDefinitionId, account.AccountId, false);
                if (childAccounts != null)
                {
                    foreach (var childAccount in childAccounts)
                    {
                        if (IsSiteMapped(childAccount))
                            return false;
                        if (IsChangedUserRGs(childAccount, false, null))
                            return false;
                    }
                }
            
                return true;
            }
            return false;
        }
        public static bool CanChangeSiteMapping(Account account, Guid siteTypeId)
        {
           if (account.TypeId == siteTypeId)
            {
                if (IsSiteNotMappedOrProvisioned(account))
                    return false;
                return !IsChangedUserRGs(account, false, null);
            }
            return false;
        }
        public static bool AllowChangeUserRGs(Account account, Guid companyTypeId, Guid siteTypeId,Guid? userTypeId, string actionType)
        {
            bool result = false;
            if (IsEnterpriseMapped(account) || IsSiteMapped(account) || IsUserMapped(account))
            {
                if (!IsChangedUserRGs(account, true, actionType))
                    result = true;
            }
           var childAccounts = _accountBEManager.GetChildAccounts(account, true);
           if (childAccounts != null)
           {
               foreach(var child in childAccounts)
               {
                   if (IsEnterpriseMapped(child) || IsSiteMapped(child) || IsUserMapped(child))
                   {
                       if (!IsChangedUserRGs(child, true, actionType))
                       {
                           result = true;
                           break;
                       }
                   }
               }
           }

           return result;
           
        }
        public static bool AllowRevertUserRGs(Account account, Guid companyTypeId, Guid siteTypeId,Guid? userTypeId, string actionType)
        {
            bool result = false;
            if (IsEnterpriseMapped(account) || IsSiteMapped(account) || IsUserMapped(account))
            {
                if (IsChangedUserRGs(account, true, actionType))
                    result = true;
            }
            var childAccounts = _accountBEManager.GetChildAccounts(account, true);
            if (childAccounts != null)
            {
                foreach (var child in childAccounts)
                {
                    if (IsEnterpriseMapped(child) || IsSiteMapped(child) || IsUserMapped(child))
                    {
                        if (IsChangedUserRGs(child, true, actionType))
                        {
                            result = true;
                            break;
                        }
                    }
                }
            }
            return result;
           
        }
        public static bool AllowEnterpriseMap(Account account, Guid companyTypeId, Guid siteTypeId)
        {
            if (account.TypeId == companyTypeId)
            {
                return !IsEnterpriseMapped(account);
            }
            return false;
        }
        public static bool AllowSiteMap(Account account, Guid companyTypeId, Guid siteTypeId)
        {
            if (account.TypeId == siteTypeId)
            {
                var parentAccount = _accountBEManager.GetParentAccount(account);
                parentAccount.ThrowIfNull("account", account.ParentAccountId.Value);
                if (!IsEnterpriseMapped(parentAccount))
                    return false;
                return !IsSiteMapped(account);
            }
            return false;
        }

        public static bool CanChangeUserMapping(Account account, Guid? userTypeId)
        {
            if (userTypeId.HasValue && account.TypeId == userTypeId)
            {
                if (IsUserNotMappedOrProvisioned(account))
                    return false;
                return !IsChangedUserRGs(account, false, null);
            }
            return false;
        }
        public static bool AllowUserMap(Account account, Guid? userTypeId)
        {
            if (userTypeId.HasValue && account.TypeId == userTypeId)
            {
                return !IsUserMapped(account);
            }
            return false;
        }

    }
}

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
    public enum ConditionType { CanChangeMapping = 0, AllowChangeUserRGs = 1, AllowRevertUserRGs = 2, AllowEnterpriseMap = 3, AllowSiteMap = 4 }
    public class TelesAccountCondition : AccountCondition
    {
        public override Guid ConfigId { get { return new Guid("2C1CEA7E-96F1-4BB0-83DD-FE8BA4BA984C"); } }
        public ConditionType ConditionType { get; set; }
        public Guid CompanyTypeId { get; set; }
        public Guid SiteTypeId { get; set; }
        public string ActionType { get; set; }

        static AccountBEManager _accountBEManager = new AccountBEManager();
        static AccountTypeManager _accountTypeManager = new AccountTypeManager();

        public override bool Evaluate(IAccountConditionEvaluationContext context)
        {
            switch(this.ConditionType)
            {
                case Business.ConditionType.CanChangeMapping:
                    return AllowChangeMapping(context.Account, this.CompanyTypeId, this.SiteTypeId);
                case Business.ConditionType.AllowChangeUserRGs:
                    return AllowChangeUserRGs(context.Account, this.CompanyTypeId, this.SiteTypeId, this.ActionType);
                case Business.ConditionType.AllowRevertUserRGs:
                    return AllowRevertUserRGs(context.Account, this.CompanyTypeId, this.SiteTypeId,this.ActionType);
                case Business.ConditionType.AllowEnterpriseMap:
                    return AllowEnterpriseMap(context.Account, this.CompanyTypeId, this.SiteTypeId);
                case Business.ConditionType.AllowSiteMap:
                    return AllowSiteMap(context.Account, this.CompanyTypeId, this.SiteTypeId);
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

        public static bool AllowChangeMapping(Account account, Guid companyTypeId, Guid siteTypeId)
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
            else if (account.TypeId == siteTypeId)
            {
                if (IsSiteNotMappedOrProvisioned(account))
                    return false;
                return !IsChangedUserRGs(account, false, null);
            }
            return false;
        }
        public static bool AllowChangeUserRGs(Account account, Guid companyTypeId, Guid siteTypeId, string actionType)
        {
            if (account.TypeId == companyTypeId)
            {
                if (!IsEnterpriseMapped(account))
                    return false;
                return !IsChangedUserRGs(account, true,actionType);
            }
            else if (account.TypeId == siteTypeId)
            {
                if (!IsSiteMapped(account))
                    return false;

                return !IsChangedUserRGs(account,true, actionType);
            }
            return false;
        }
        public static bool AllowRevertUserRGs(Account account, Guid companyTypeId, Guid siteTypeId,string actionType)
        {
            if (account.TypeId == companyTypeId)
            {
                return IsChangedUserRGs(account,true, actionType);
            }
            else if (account.TypeId == siteTypeId)
            {
                return IsChangedUserRGs(account,true, actionType);
            }
            return false;
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
    }
}

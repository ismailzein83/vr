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
    public enum ConditionType { CanChangeMapping = 0, AlowBlockInternational = 1,AllowUnBlockInternational=2 }
    public class TelesAccountCondition : AccountCondition
    {
        public override Guid ConfigId { get { return new Guid("2C1CEA7E-96F1-4BB0-83DD-FE8BA4BA984C"); } }
        public ConditionType ConditionType { get; set; }
        public Guid CompanyTypeId { get; set; }
        public Guid SiteTypeId { get; set; }
        public string ActionType { get; set; }

        AccountBEManager _accountBEManager = new AccountBEManager();
        AccountTypeManager _accountTypeManager = new AccountTypeManager();

        public override bool Evaluate(IAccountConditionEvaluationContext context)
        {
            switch(this.ConditionType)
            {
                case Business.ConditionType.CanChangeMapping:
                    if(context.Account.TypeId == this.CompanyTypeId)
                    {
                        if (IsEnterpriseNotMappedOrProvisioned(context.Account))
                            return false;
                        if (IsInternationalCallsBlocked(context.Account))
                            return false;

                        var accountBEDefinitionId = _accountTypeManager.GetAccountBEDefinitionId(context.Account.TypeId);
                        var accountChilds = _accountBEManager.GetChildAccounts(accountBEDefinitionId, context.Account.AccountId, false);
                        foreach(var account in accountChilds)
                        {
                            if (IsSiteMapped(account))
                                return false;
                            if (IsInternationalCallsBlocked(account))
                                return false;
                        }
                        return true;
                    }
                    else if (context.Account.TypeId == this.SiteTypeId)
                    {
                        if (IsSiteNotMappedOrProvisioned(context.Account))
                            return false;
                        return !IsInternationalCallsBlocked(context.Account);
                    }
                    return false;
                case Business.ConditionType.AlowBlockInternational:
                    if (context.Account.TypeId == this.CompanyTypeId)
                    {
                        if (!IsEnterpriseMapped(context.Account))
                            return false;
                        return !IsInternationalCallsBlocked(context.Account);
                    }
                    else if (context.Account.TypeId == this.SiteTypeId)
                    {
                        if (!IsSiteMapped(context.Account))
                            return false;

                        return !IsInternationalCallsBlocked(context.Account);
                    }
                    return false;
                case Business.ConditionType.AllowUnBlockInternational:
                     if (context.Account.TypeId == this.CompanyTypeId)
                    {
                        return IsInternationalCallsBlocked(context.Account);
                    }
                    else if (context.Account.TypeId == this.SiteTypeId)
                    {
                        return IsInternationalCallsBlocked(context.Account);
                    }
                    return false;
                default:
                    return false;
            }
        }
        private bool IsEnterpriseNotMappedOrProvisioned(Account account)
        {
            var enterpriseAccountMappingInfo = _accountBEManager.GetExtendedSettings<EnterpriseAccountMappingInfo>(account);
            return enterpriseAccountMappingInfo == null || enterpriseAccountMappingInfo.Status.HasValue;
        }
        private bool IsSiteNotMappedOrProvisioned(Account account)
        {
            var siteAccountMappingInfo = _accountBEManager.GetExtendedSettings<SiteAccountMappingInfo>(account);
            return siteAccountMappingInfo == null || siteAccountMappingInfo.Status.HasValue;
        }
        private bool IsEnterpriseMapped(Account account)
        {
            var enterpriseAccountMappingInfo = _accountBEManager.GetExtendedSettings<EnterpriseAccountMappingInfo>(account);
            return enterpriseAccountMappingInfo != null;
        }
        private bool IsSiteMapped(Account account)
        {
            var siteAccountMappingInfo = _accountBEManager.GetExtendedSettings<SiteAccountMappingInfo>(account);
            return siteAccountMappingInfo != null;
        }
        private bool IsInternationalCallsBlocked(Account account)
        {
            this.ActionType.ThrowIfNull("ActionType");
            var changeUsersRGsAccountState = _accountBEManager.GetExtendedSettings<ChangeUsersRGsAccountState>(account);
            if (changeUsersRGsAccountState != null && changeUsersRGsAccountState.ChangesByActionType != null)
            {
                ChURGsActionCh chURGsActionCh;
                if(changeUsersRGsAccountState.ChangesByActionType.TryGetValue(this.ActionType,out chURGsActionCh))
                {
                    if (chURGsActionCh.Status == ChURGsActionChStatus.Blocked)
                        return true;
                }
            }
            return false;
        }
    }
}

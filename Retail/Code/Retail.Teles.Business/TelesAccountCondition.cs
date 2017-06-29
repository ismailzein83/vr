using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using Retail.Teles.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Teles.Business
{
    public enum ConditionType { CanChangeMapping = 0 }
    public class TelesAccountCondition : AccountCondition
    {
        public override Guid ConfigId { get { return new Guid("2C1CEA7E-96F1-4BB0-83DD-FE8BA4BA984C"); } }
        public ConditionType ConditionType { get; set; }
        public Guid CompanyTypeId { get; set; }
        public Guid SiteTypeId { get; set; }

        AccountBEManager _accountBEManager = new AccountBEManager();
        AccountTypeManager _accountTypeManager = new AccountTypeManager();

        public override bool Evaluate(IAccountConditionEvaluationContext context)
        {
            switch(this.ConditionType)
            {
                case Business.ConditionType.CanChangeMapping:
                    if(context.Account.TypeId == this.CompanyTypeId)
                    {
                        var enterpriseAccountMappingInfo = _accountBEManager.GetExtendedSettings<EnterpriseAccountMappingInfo>(context.Account);
                        if (enterpriseAccountMappingInfo == null || enterpriseAccountMappingInfo.Status.HasValue)
                        {
                            return false;
                        }
                        var changeUsersRGsAccountState = _accountBEManager.GetExtendedSettings<ChangeUsersRGsAccountState>(context.Account);
                        if (changeUsersRGsAccountState != null)
                            return false;
                        var accountBEDefinitionId = _accountTypeManager.GetAccountBEDefinitionId(context.Account.TypeId);
                        var accountChilds = _accountBEManager.GetChildAccounts(accountBEDefinitionId, context.Account.AccountId, false);
                        foreach(var account in accountChilds)
                        {
                            var childChangeUsersRGsAccountState = _accountBEManager.GetExtendedSettings<ChangeUsersRGsAccountState>(account);
                            if (childChangeUsersRGsAccountState != null)
                                return false;
                        }
                    }else
                    {
                        var siteAccountMappingInfo = _accountBEManager.GetExtendedSettings<SiteAccountMappingInfo>(context.Account);
                        if (siteAccountMappingInfo == null || siteAccountMappingInfo.Status.HasValue)
                        {
                            return false;
                        }
                        var changeUsersRGsAccountState = _accountBEManager.GetExtendedSettings<ChangeUsersRGsAccountState>(context.Account);
                        if (changeUsersRGsAccountState != null)
                            return false;

                    }
                    return true;
                default:
                    return false;
            }
        }
    }
}

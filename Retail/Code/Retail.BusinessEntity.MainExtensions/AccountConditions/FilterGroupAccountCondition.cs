using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Retail.BusinessEntity.MainExtensions.AccountConditions
{
    public class FilterGroupAccountCondition : AccountCondition
    {
        public override Guid ConfigId { get { return new Guid("1B1AF5DD-52EB-42C7-97EF-8CE824BB7D03"); } }

        public RecordFilterGroup FilterGroup { get; set; }

        static AccountBEManager s_accountBEManager = new AccountBEManager();
        public override bool Evaluate(IAccountConditionEvaluationContext context)
        {
            return s_accountBEManager.IsAccountMatchWithFilterGroup(context.Account, this.FilterGroup);
        }
    }

    public class FilterGroupAccountServiceCondition : AccountServiceCondition
    {
        public Dictionary<Guid, ServiceTypeFilterGroup> ServiceTypeFilterGroups { get; set; }

        static AccountServiceManager s_accountServiceManager = new AccountServiceManager();
        public override bool Evaluate(IAccountServiceConditionEvaluationContext context)
        {
            ServiceTypeFilterGroup serviceTypeFilterGroup;
            if (this.ServiceTypeFilterGroups.TryGetValue(context.AccountService.ServiceTypeId, out serviceTypeFilterGroup))
            {
                if (serviceTypeFilterGroup.FilterGroup == null 
                    || s_accountServiceManager.IsAccountServiceMatchWithFilterGroup(context.Account, context.AccountService, serviceTypeFilterGroup.FilterGroup))
                    return true;
                else
                    return false;
            }
            else
                return false;
        }
    }

    public class ServiceTypeFilterGroup
    {
        public RecordFilterGroup FilterGroup { get; set; }
    }

}

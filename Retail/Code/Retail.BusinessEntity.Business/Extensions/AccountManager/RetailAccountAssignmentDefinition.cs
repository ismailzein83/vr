using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountManager.Entities;

namespace Retail.BusinessEntity.Business
{
    public class RetailAccountAssignmentDefinition : AccountManagerAssignmentDefinitionSettings
    {
        public override string GetAccountName(string accountId)
        {
            throw new NotImplementedException();
        }
        public Guid AccountBEDefinitionId { get; set; }
        public Retail.BusinessEntity.Entities.AccountCondition AccountCondition { get; set; }
    }
}

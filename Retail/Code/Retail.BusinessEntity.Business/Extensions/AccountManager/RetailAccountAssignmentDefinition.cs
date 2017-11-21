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
            AccountBEManager accountBeManager = new AccountBEManager();
            return accountBeManager.GetAccountName(this.AccountBEDefinitionId, Convert.ToInt64(accountId));
        }
        public Guid AccountBEDefinitionId { get; set; }
        public Retail.BusinessEntity.Entities.AccountCondition AccountCondition { get; set; }

        public override Guid ConfigId
        {
            get { return new Guid("4CC58F9E-5ED1-4C12-8E3E-E38FC19DFF53"); }
        }

        public override string RuntimeEditor
        {
            get
            {
                return "retail-be-accountassignment-runtime";
            }
        }
    }
}

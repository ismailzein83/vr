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

        public override Guid ConfigId
        {
            get { return new Guid("5592F2FF-09CB-4BE0-A534-CCBB7631B00B"); }
        }

        public override string RuntimeEditor
        {
            get
            {
                return "";
            }
        }
    }
}

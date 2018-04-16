using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.BP.Arguments
{
    public class AccountBulkActionProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public Guid AccountBEDefinitionId { get; set; }
        public Guid BulkActionIdentifier { get; set; }
        public List<AccountBulkActionRuntime> AccountBulkActions { get; set; }
        public HandlingErrorOption HandlingErrorOption { get; set; }
        public Vanrise.Entities.BulkActionFinalState BulkActionFinalState { get; set; }
        public override string GetTitle()
        {
            return "Account Bulk Action Process";
        }
    }
}

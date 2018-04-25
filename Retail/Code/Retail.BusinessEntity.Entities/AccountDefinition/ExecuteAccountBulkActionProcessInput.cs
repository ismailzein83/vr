using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class ExecuteAccountBulkActionProcessInput
    {
        public Guid AccountBEDefinitionId { get; set; }
        public List<AccountBulkActionRuntime> AccountBulkActions { get; set; }
        public HandlingErrorOption HandlingErrorOption { get; set; }
        public Vanrise.Entities.BulkActionFinalState BulkActionFinalState { get; set; }
    }
}

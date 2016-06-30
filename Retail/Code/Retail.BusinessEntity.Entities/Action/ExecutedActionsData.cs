using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class ExecutedActionsData
    {
        public List<ExecutedActionData> ExecutedActionData { get; set; }
    }

    public class ExecutedActionData
    {
        public Guid ActionDefinitionId { get; set; }

        public DateTime ExecutionTime { get; set; }

        public Object ProvisioningData { get; set; }
    }
}

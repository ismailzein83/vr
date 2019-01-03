using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Business
{
    public class ExecuteGenericBEBulkActionProcessInput
    {
        public Guid GenericBEDefinitionId { get; set; }
        public List<GenericBEBulkActionRuntime> GenericBEBulkActions { get; set; }
        public HandlingErrorOption HandlingErrorOption { get; set; }
        public Vanrise.Entities.BulkActionFinalState BulkActionFinalState { get; set; }
    }
}

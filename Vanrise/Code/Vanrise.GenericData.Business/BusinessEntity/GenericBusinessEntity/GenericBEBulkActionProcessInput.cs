using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Business
{
    public enum HandlingErrorOption { Skip = 1, Stop = 2 }
    public class GenericBEBulkActionProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public Guid BEDefinitionId { get; set; }
        public List<GenericBEBulkActionRuntime> BulkActions { get; set; }
        public HandlingErrorOption HandlingErrorOption { get; set; }
        public Vanrise.Entities.BulkActionFinalState BulkActionFinalState { get; set; }
        public override string GetTitle()
        {
            return "Bulk Action Process";
        }
    }
    public class GenericBEBulkActionRuntime
    {
        public Guid AccountBulkActionId { get; set; }
        public GenericBEBulkActionRuntimeSettings Settings { get; set; }
    }
}

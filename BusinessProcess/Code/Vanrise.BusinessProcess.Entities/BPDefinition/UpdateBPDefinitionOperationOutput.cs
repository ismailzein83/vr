using System.Collections.Generic;

namespace Vanrise.BusinessProcess.Entities
{
    public class UpdateBPDefinitionOperationOutput : Vanrise.Entities.UpdateOperationOutput<BPDefinitionDetail>
    {
        public List<string> ValidationMessages { get; set; }
    }
}

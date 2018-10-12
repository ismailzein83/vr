using System.Collections.Generic;

namespace Vanrise.BusinessProcess.Entities
{
    public class InsertBPDefinitionOperationOutput : Vanrise.Entities.InsertOperationOutput<BPDefinitionDetail>
    {
        public List<string> ValidationMessages { get; set; }
    }
}

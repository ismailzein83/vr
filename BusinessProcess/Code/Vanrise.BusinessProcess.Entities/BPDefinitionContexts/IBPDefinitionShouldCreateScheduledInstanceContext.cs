using System;

namespace Vanrise.BusinessProcess.Entities
{
    public interface IBPDefinitionShouldCreateScheduledInstanceContext
    {
        BaseProcessInputArgument BaseProcessInputArgument { get; }
    }

    public class BPDefinitionShouldCreateScheduledInstanceContext : IBPDefinitionShouldCreateScheduledInstanceContext
    {
        public BaseProcessInputArgument BaseProcessInputArgument { get; set; }
    }
}

using System;
using Vanrise.Common;
using Vanrise.Reprocess.Entities;

namespace Vanrise.Reprocess.Business
{
    public class ManualTriggerReprocessDefinitionFilter : IReprocessDefinitionFilter
    {
        public bool IsMatched(IReprocessDefinitionFilterContext context)
        {
            context.ReprocessDefinition.ThrowIfNull("context.ReprocessDefinition");
            context.ReprocessDefinition.Settings.ThrowIfNull("context.ReprocessDefinition.Settings");
            if (context.ReprocessDefinition.Settings.CannotBeTriggeredManually)
                return false;

            return true;
        }
    }
}
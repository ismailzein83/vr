using System;
using System.Activities;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Reprocess.Entities;

namespace Vanrise.Reprocess.BP.Activities
{
    public sealed class ValidateReprocessInput : CodeActivity
    {
        [RequiredArgument]
        public InArgument<ReprocessDefinition> ReprocessDefinition { get; set; }

        [RequiredArgument]
        public InArgument<Guid> ReprocessDefinitionId { get; set; }

        [RequiredArgument]
        public InArgument<bool> UseTempStorageInput { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            Guid reprocessDefinitionId = this.ReprocessDefinitionId.Get(context);
            ReprocessDefinition reprocessDefinition = this.ReprocessDefinition.Get(context);
            bool useTempStorageInput = this.UseTempStorageInput.Get(context);

            reprocessDefinition.ThrowIfNull("reprocessDefinition", reprocessDefinitionId);
            reprocessDefinition.Settings.ThrowIfNull("reprocessDefinition.Settings", reprocessDefinitionId);

            if (reprocessDefinition.Settings.ForceUseTempStorage && !useTempStorageInput)
                throw new DataIntegrityValidationException(string.Format("Temporay Storage must be used for the Reprocess {0}", reprocessDefinition.Name));
        }
    }
}
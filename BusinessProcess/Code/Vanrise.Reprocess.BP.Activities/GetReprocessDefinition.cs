using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Reprocess.Entities;
using Vanrise.Reprocess.Business;

namespace Vanrise.Reprocess.BP.Activities
{

    public sealed class GetReprocessDefinition : CodeActivity
    {
        public InArgument<Guid> ReprocessDefinitionId { get; set; }

        public OutArgument<ReprocessDefinition> ReprocessDefinition { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            ReprocessDefinitionManager manager = new ReprocessDefinitionManager();
            ReprocessDefinition reprocessDefintion = manager.GetReprocessDefinition(this.ReprocessDefinitionId.Get(context));

            this.ReprocessDefinition.Set(context, reprocessDefintion);
        }
    }
}

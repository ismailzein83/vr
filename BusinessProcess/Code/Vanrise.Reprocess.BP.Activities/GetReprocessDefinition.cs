using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Reprocess.Entities;

namespace Vanrise.Reprocess.BP.Activities
{

    public sealed class GetReprocessDefinition : CodeActivity
    {
        public InArgument<int> ReprocessDefinitionId { get; set; }

        public OutArgument<ReprocessDefinition> ReprocessDefinition { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            throw new NotImplementedException();
        }
    }
}

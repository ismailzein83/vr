using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Mediation.Generic.Entities;
using Mediation.Generic.Business;
using Vanrise.Common;

namespace Mediation.Generic.BP.Activities
{

    public sealed class GetMediationDefinition : CodeActivity
    {
        [RequiredArgument]
        public InArgument<int> MediationDefinitionId { get; set; }

        [RequiredArgument]
        public InOutArgument<MediationDefinition> MediationDefinition { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            int mediationDefinitionId = MediationDefinitionId.Get(context);
            MediationDefinitionManager mediationDefinitionManager = new MediationDefinitionManager();
            var mediationDefinition = mediationDefinitionManager.GetMediationDefinition(mediationDefinitionId);
            mediationDefinition.ThrowIfNull("mediationDefinition", mediationDefinitionId);
            mediationDefinition.OutputHandlers.ThrowIfNull("mediationDefinition.OutputHandlers", mediationDefinitionId);
            MediationDefinition.Set(context, mediationDefinition);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Mediation.Generic.Entities;
using Mediation.Generic.Business;

namespace Mediation.Generic.BP.Activities
{

    public sealed class GetMediationDefinition : CodeActivity
    {
        [RequiredArgument]
        public InArgument<int> MediationDefinitionId { get; set; }

        [RequiredArgument]
        public OutArgument<MediationDefinition> MediationDefinition { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            MediationDefinitionManager mediationDefinitionManager = new MediationDefinitionManager();
            var mediationDefinition = mediationDefinitionManager.GetMediationDefinition(MediationDefinitionId.Get(context));
            if (mediationDefinition == null)
                throw new NullReferenceException("GetMediationDefinition: mediationDefinition");
            MediationDefinition.Set(context, mediationDefinition);
        }
    }
}

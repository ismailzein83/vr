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
        public InOutArgument<MediationDefinition> MediationDefinition { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            int mediationDefinitionId = MediationDefinitionId.Get(context);
            MediationDefinitionManager mediationDefinitionManager = new MediationDefinitionManager();
            var mediationDefinition = mediationDefinitionManager.GetMediationDefinition(mediationDefinitionId);
            if (mediationDefinition == null)
                throw new NullReferenceException(string.Format("GetMediationDefinition: mediationDefinitionId {0}", mediationDefinitionId));
            MediationDefinition.Set(context, mediationDefinition);
        }
    }
}

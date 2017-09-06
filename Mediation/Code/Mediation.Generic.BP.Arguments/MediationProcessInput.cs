using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mediation.Generic.Entities;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;

namespace Mediation.Generic.BP.Arguments
{
    public class MediationProcessInput : BaseProcessInputArgument
    {
        public Guid MediationDefinitionId { get; set; }

        public override string GetTitle()
        {
            IMediationDefinitionManager mediationManager = MediationManagerFactory.GetManager<IMediationDefinitionManager>();
            MediationDefinition mediationDefinition = mediationManager.GetMediationDefinition(MediationDefinitionId);
            mediationDefinition.ThrowIfNull("mediationDefinition");
            return mediationDefinition.Name;
        }
    }
}

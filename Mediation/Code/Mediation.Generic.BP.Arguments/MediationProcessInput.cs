using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace Mediation.Generic.BP.Arguments
{
    public class MediationProcessInput : BaseProcessInputArgument
    {
        public Guid MediationDefinitionId { get; set; }

        public override string GetTitle()
        {
            return "Mediation Process";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class GenericRuleViewSettings : Vanrise.Security.Entities.ViewSettings
    {
        public int RuleDefinitionId { get; set; }

        public override string GetURL(Security.Entities.View view)
        {
            return String.Format("#/view/VR_GenericData/Views/GenericRule/GenericRuleManagement/{{\"ruleDefinitionId\":\"{0}\"}}", this.RuleDefinitionId);
        }
    }
}

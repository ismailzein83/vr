using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRRuleDefinitionSettings : Vanrise.Entities.VRComponentTypeSettings
    {
        public override Guid VRComponentTypeConfigId { get { return new Guid("3CC81591-9860-4B9D-9755-0E5D7E87A596"); } }

        public VRRuleDefinitionExtendedSettings VRRuleDefinitionExtendedSettings { get; set; }
    }
}

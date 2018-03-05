using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public class QualityConfigurationDefinitionSettings : Vanrise.Entities.VRComponentTypeSettings
    {
        public override Guid VRComponentTypeConfigId { get { return new Guid("B69F4C1B-FAE8-4352-BB9F-A5AA140F30EE"); } }

        public QualityConfigurationDefinitionExtendedSettings ExtendedSettings { get; set; }
    }
}
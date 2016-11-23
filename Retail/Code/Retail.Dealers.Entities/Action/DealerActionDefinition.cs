using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Dealers.Entities
{
    public class DealerActionDefinition : Vanrise.Entities.VRComponentType<DealerActionDefinitionSettings>
    {
    }

    public class DealerActionDefinitionSettings : Vanrise.Entities.VRComponentTypeSettings
    {
        public override Guid VRComponentTypeConfigId
        {
            get { return new Guid("C16C3370-1053-4B50-B23C-8C967AC830BC"); }
        }

        public string Description { get; set; }

        public DealerCondition AvailabilityCondition { get; set; }

        public DealerActionBPDefinitionSettings BPDefinitionSettings { get; set; }
    }

    public class DealerActionStatusDefinition
    {
        public Guid StatusDefinitionId { get; set; }
    }
}

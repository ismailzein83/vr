using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{    
    public class StatusDefinition
    {
        public Guid StatusDefinitionId { get; set; }

        public string Name { get; set; }
        
        public StatusDefinitionSettings Settings { get; set; }

        public EntityType EntityType { get; set; }

        public Guid StatusGroupDefinitionId { get; set; }
    }

    public class StatusDefinitionSettings
    {
        public Guid StyleDefinitionId { get; set; }

        public bool HasInitialCharge { get; set; }

        public bool HasRecurringCharge { get; set; }
    }

    public class StatusGroupDefinition : Vanrise.Entities.VRComponentType<StatusGroupDefinitionSettings>
    {

    }

    public class StatusGroupDefinitionSettings : Vanrise.Entities.VRComponentTypeSettings
    {
        public override Guid VRComponentTypeConfigId
        {
            get { return new Guid("4D881B3B-58AF-4EB0-B963-2CCFD1646BA2"); }
        }
    }
} 
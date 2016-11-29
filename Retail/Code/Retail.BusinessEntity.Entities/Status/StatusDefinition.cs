using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{    
    public class StatusDefinition
    {
        public const string BUSINESSENTITY_DEFINITION_NAME = "Retail_BE_StatusDefinition";
        public static Guid BUSINESSENTITY_DEFINITION_ID = new Guid("1B6FABEA-102E-42D7-A3BB-D037965DF3C2");

        public Guid StatusDefinitionId { get; set; }

        public string Name { get; set; }
        
        public StatusDefinitionSettings Settings { get; set; }

        public EntityType EntityType { get; set; }
    }

    public class StatusDefinitionSettings
    {
        public Guid StyleDefinitionId { get; set; }

        public bool HasInitialCharge { get; set; }

        public bool HasRecurringCharge { get; set; }
    }
} 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRObjectTypeDefinition
    {
        public Guid VRObjectTypeDefinitionId { get; set; }

        public string Name { get; set; }

        public VRObjectTypeDefinitionSettings Settings { get; set; }
    }

    public class VRObjectTypeDefinitionSettings
    {
        public VRObjectType ObjectType { get; set; }

        public Dictionary<string, VRObjectTypePropertyDefinition> Properties { get; set; }
    }

    public class VRObjectTypePropertyDefinition
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public VRObjectPropertyEvaluator PropertyEvaluator { get; set; }
    }
}

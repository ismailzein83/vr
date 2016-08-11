using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRObjectVariable
    {
        public string ObjectName { get; set; }

        public Guid VRObjectTypeDefinitionId { get; set; }

        public VRObjectType ObjectType { get; set; }
    }

    public class VRObjectVariableCollection : Dictionary<string, VRObjectVariable>
    {

    }
}

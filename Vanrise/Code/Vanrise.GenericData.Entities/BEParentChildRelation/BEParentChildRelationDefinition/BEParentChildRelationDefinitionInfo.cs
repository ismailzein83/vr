using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class BEParentChildRelationDefinitionInfo
    {
        public Guid BEParentChildRelationDefinitionId { get; set; }

        public string Name { get; set; }

        public Guid ParentBEDefinitionId { get; set; }

        public Guid ChildBEDefinitionId { get; set; }
    }
}

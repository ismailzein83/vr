using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class BEParentChildRelationDefinitionFilter
    {
        public Guid ParentBEDefinitionId { get; set; }

        public Guid ChildBEDefinitionId { get; set; }
    }
}

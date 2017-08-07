using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class BEParentChildrenRelation 
    {
        public long BEParentChildRelationId { get; set; }

        public Guid RelationDefinitionId { get; set; }

        public string ParentBEId { get; set; }

        public List<string> ChildBEIds { get; set; } 

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
    }
}

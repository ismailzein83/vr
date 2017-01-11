using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class BEParentChildRelation
    {
        public long BEParentChildRelationId { get; set; }

        public Guid RelationDefinitionId { get; set; }

        public string ParentBEId { get; set; }

        public string ChildBEId { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class BEParentChildRelationQuery
    {
        public Guid RelationDefinitionId { get; set; }

        public string ParentBEId { get; set; }

        public string ChildBEId { get; set; }
    }
}

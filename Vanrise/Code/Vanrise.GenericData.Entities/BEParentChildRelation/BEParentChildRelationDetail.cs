using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class BEParentChildRelationDetail
    {
        public BEParentChildRelation Entity { get; set; }

        public string ParentBEName { get; set; }

        public string ChildBEName { get; set; }
    }
}

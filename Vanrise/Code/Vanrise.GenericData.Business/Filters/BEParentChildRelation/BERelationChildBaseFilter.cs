using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Business
{
    public class BERelationChildBaseFilter
    {
        public Guid ParentChildRelationDefinition { get; set; }

        public bool IsChildExcluded(object childId)
        {
            IEnumerable<BEParentChildRelation> beParentChildRelations = new BEParentChildRelationManager().GetBEParentChildRelationsByDefinitionId(this.ParentChildRelationDefinition);
            IEnumerable<string> assignedChildrens = beParentChildRelations.Select(itm => (itm.ChildBEId.ToLower()));

            if (assignedChildrens != null && assignedChildrens.Contains(childId.ToString().ToLower()))
                return false;

            return true;
        }
    }
}

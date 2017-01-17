using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Common;

namespace Vanrise.GenericData.Business
{
    public class BERelationChildBaseFilter
    {
        public Guid ParentChildRelationDefinition { get; set; }

        public bool IsChildExcluded(object childId)
        {
            IEnumerable<BEParentChildRelation> beParentChildRelations = new BEParentChildRelationManager().GetBEParentChildRelationsByDefinitionId(this.ParentChildRelationDefinition);
            string childIdAsString = childId.ToString();

            if (beParentChildRelations.FindRecord(itm => string.Compare(itm.ChildBEId, childIdAsString, true) == 0) != null)
                return false;

            return true;
        }
    }
}

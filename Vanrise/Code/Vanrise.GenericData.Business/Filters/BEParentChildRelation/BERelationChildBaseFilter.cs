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
        static BEParentChildRelationManager _beParentChildRelationManager = new BEParentChildRelationManager();
        
        public Guid ParentChildRelationDefinitionId { get; set; }

        public bool IsChildExcluded(object childId)
        {
            List<BEParentChildRelation> beParentChildRelations = _beParentChildRelationManager.GetParent(ParentChildRelationDefinitionId, childId.ToString());

            if (beParentChildRelations != null && beParentChildRelations.Count != 0)
                return false;

            return true;
        }
    }
}

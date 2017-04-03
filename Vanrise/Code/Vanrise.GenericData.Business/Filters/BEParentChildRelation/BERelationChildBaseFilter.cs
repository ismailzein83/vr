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
        static BEParentChildRelationManager s_beParentChildRelationManager = new BEParentChildRelationManager();
        
        public Guid ParentChildRelationDefinitionId { get; set; }

        public bool IsChildExcluded(object childId)
        {
            if (s_beParentChildRelationManager.IsChildAssignedToParentWithoutEED(this.ParentChildRelationDefinitionId, childId.ToString()))
                return true;

            return false;
        }
    }
}

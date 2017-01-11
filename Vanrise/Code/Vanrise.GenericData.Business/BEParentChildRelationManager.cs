using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Business
{
    public class BEParentChildRelationManager
    {
        public BEParentChildRelation GetParent(Guid beParentChildRelationDefinitionId, string childId, DateTime effectiveOn)
        {
            throw new NotImplementedException();
        }

        public List<BEParentChildRelation> GetChildren(Guid beParentChildRelationDefinitionId, string parentId, DateTime effectiveOn)
        {
            throw new NotImplementedException();
        }
    }
}

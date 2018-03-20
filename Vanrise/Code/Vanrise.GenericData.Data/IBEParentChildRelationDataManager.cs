using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Data
{
    public interface IBEParentChildRelationDataManager : IDataManager
    {
        List<BEParentChildRelation> GetBEParentChildRelations(Guid beParentChildRelationDefinitionId);

        bool AreBEParentChildRelationUpdated(Guid beParentChildRelationDefinitionId, ref object updateHandle);

        bool Insert(BEParentChildRelation BEParentChildRelationItem, out long insertedId);

        bool Update(BEParentChildRelation BEParentChildRelationItem);
    }
}

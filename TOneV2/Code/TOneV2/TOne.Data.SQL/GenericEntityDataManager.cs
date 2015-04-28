using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Entities;

namespace TOne.Data.SQL
{
    public class GenericEntityDataManager : BaseTOneDataManager
    {
        public GenericEntityDataManager()
            : base("ConfigDBConnString")
        {
        }

        public bool InsertEntity(GenericEntity entity)
        {
            int typeId = GetEntityTypeId(entity.GetType().AssemblyQualifiedName);
            int baseTypeId = GetEntityTypeId(entity.BaseType.AssemblyQualifiedName);
            object id;
            if (ExecuteNonQuerySP("generic.sp_GenericEntity_Insert", out id, entity.ParentId, entity.OwnerId, entity.LinkedToEntityId,
                typeId, baseTypeId, (int)GenericEntityStatus.Active, Vanrise.Common.Serializer.Serialize(entity)) > 0)
            {
                entity.EntityId = (int)id;
                return true;
            }
            else
                return false;
        }

        public int GetEntityTypeId(string fqtn)
        {
            return (int)ExecuteScalarSP("generic.sp_GenericEntityType_InsertAndGetID", fqtn);
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Data
{
    public interface IGenericBusinessEntityDataManager:IDataManager
    {
        List<GenericBusinessEntity> GetGenericBusinessEntitiesByDefinition(int businessDefinitionId);
        bool AddGenericBusinessEntity(GenericBusinessEntity genericBusinessEntity, out long genericBusinessEntityId);
        bool UpdateGenericBusinessEntity(GenericBusinessEntity genericBusinessEntity);
        bool DeleteGenericBusinessEntity(long genericBusinessEntityId, int businessEntityDefinitionId);
        bool AreGenericBusinessEntityUpdated(int parameter, ref object updateHandle);
    }
}

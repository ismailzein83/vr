﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public interface IBusinessEntityDefinitionManager : IBusinessManager
    {
        string GetBusinessEntityDefinitionName(Guid businessEntityDefinitionId);

        BusinessEntityDefinition GetBusinessEntityDefinition(Guid businessEntityDefinitionId);
    }
}

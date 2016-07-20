﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public interface IActionDefinitionManager : IBEManager
    {
        ActionBPDefinitionSettings GetActionBPDefinitionSettings(Guid actionDefinitionId);
        ActionDefinition GetActionDefinition(Guid actionDefinitionId);
    }
}

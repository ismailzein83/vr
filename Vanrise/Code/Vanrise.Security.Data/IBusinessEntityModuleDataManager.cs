﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Data
{
    public interface IBusinessEntityModuleDataManager : IDataManager
    {
        List<Vanrise.Security.Entities.BusinessEntityModule> GetModules();

        bool ToggleBreakInheritance(string entityId);
    }
}

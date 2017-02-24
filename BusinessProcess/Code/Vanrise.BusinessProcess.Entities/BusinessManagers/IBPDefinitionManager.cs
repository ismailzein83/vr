﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Entities
{
    public interface IBPDefinitionManager : IBusinessManager
    {
        bool DoesUserHaveViewAccess(int userId);

        string GetDefinitionTitle(string processName);
    }
}

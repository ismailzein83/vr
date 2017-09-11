﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public interface IAnalyticTableManager : IBEManager
    {
        bool DoesUserHaveAccess(int userId, Guid analyticTableId);
    }
}

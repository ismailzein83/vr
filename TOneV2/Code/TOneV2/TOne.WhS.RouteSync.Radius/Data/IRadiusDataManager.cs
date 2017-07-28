﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Entities;
using Vanrise.Data;

namespace TOne.WhS.RouteSync.Radius
{
    public interface IRadiusDataManager : IBulkApplyDataManager<ConvertedRoute>
    {
        Guid ConfigId { get; }
        void PrepareTables(ISwitchRouteSynchronizerInitializeContext context);
        Object PrepareDataForApply(List<ConvertedRoute> radiusRoutes);
        void ApplySwitchRouteSyncRoutes(ISwitchRouteSynchronizerApplyRoutesContext context);
        void SwapTables(ISwapTableContext context);
    }
}

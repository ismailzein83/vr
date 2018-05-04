﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Entities;
using TOne.WhS.RouteSync.Ericsson.Entities;
using Vanrise.Data;

namespace TOne.WhS.RouteSync.Ericsson.Data
{
    public interface IRouteCaseDataManager : IDataManager, IBulkApplyDataManager<RouteCase>
    {
        string SwitchId { get; set; }
        IEnumerable<RouteCase> GetAllRouteCases();
        void Initialize(IRouteCaseInitializeContext context);
        void ApplyRouteCaseForDB(object preparedRouteCase);
        Dictionary<string, RouteCase> GetRouteCasesAfterRCNumber(int rcNumber);
		void UpdateSyncedRouteCases(IEnumerable<int> rCNumbers);
	}
}
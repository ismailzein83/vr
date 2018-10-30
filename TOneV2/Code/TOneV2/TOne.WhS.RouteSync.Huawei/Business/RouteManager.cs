﻿using System;
using System.Collections.Generic;
using TOne.WhS.RouteSync.Huawei.Data;
using TOne.WhS.RouteSync.Huawei.Entities;

namespace TOne.WhS.RouteSync.Huawei.Business
{
    public class RouteManager
    {
        string _switchId;
        IRouteDataManager _dataManager;

        public RouteManager(string switchId)
        {
            _switchId = switchId;

            _dataManager = RouteSyncHuaweiDataManagerFactory.GetDataManager<IRouteDataManager>();
            _dataManager.SwitchId = switchId;
        }

        public void Initialize()
        {
            _dataManager.Initialize(new RouteInitializeContext());
        }

        public void CompareTables(IRouteCompareTablesContext context)
        {
            _dataManager.CompareTables(context);
        }

        public void InsertRoutesToTempTable(IEnumerable<HuaweiConvertedRoute> routes)
        {
            _dataManager.InsertRoutesToTempTable(routes);
        }

        public void RemoveRoutesFromTempTable(IEnumerable<HuaweiConvertedRoute> routes)
        {
            _dataManager.RemoveRoutesFromTempTable(routes);
        }

        public void UpdateRoutesInTempTable(IEnumerable<HuaweiConvertedRoute> routes)
        {
            _dataManager.UpdateRoutesInTempTable(routes);
        }

        public void Finalize(IRouteFinalizeContext context)
        {
            _dataManager.Finalize(context);
        }
    }
}
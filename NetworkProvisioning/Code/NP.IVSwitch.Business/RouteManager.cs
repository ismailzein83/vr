using NP.IVSwitch.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common;
using NP.IVSwitch.Data;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common.Business;

namespace NP.IVSwitch.Business
{
    public class RouteManager
    {
        #region Public Methods

        public Route GetRouteHistoryDetailbyHistoryId(int routeHistoryId)
        {
            VRObjectTrackingManager s_vrObjectTrackingManager = new VRObjectTrackingManager();
            var route = s_vrObjectTrackingManager.GetObjectDetailById(routeHistoryId);
            return route.CastWithValidate<Route>("Route : historyId ", routeHistoryId);
        }

        public Route GetRoute(int routeId, bool isViewedFromUI)
        {
            Dictionary<int, Route> cachedRoute = this.GetCachedRoutes();
            var route = cachedRoute.GetRecord(routeId);
            if (route != null && isViewedFromUI)
                VRActionLogger.Current.LogObjectViewed(RouteLoggableEntity.Instance, route);
            return route;
        }

        public Route GetRoute(int routeId)
        {
            return GetRoute(routeId, false);
        }

        public string GetRouteDescription(Route route)
        {
            return route.Host;
        }
        public string GetRouteDescription(int routeId)
        {
            var route = GetRoute(routeId);
            return route != null ? GetRouteDescription(route) : null;
        }
        public IEnumerable<RouteEntityInfo> GetRoutesInfo(RouteInfoFilter filter)
        {
            Func<Route, bool> filterFunc = null;
            var allRoutes = this.GetCachedRoutes();
            if (filter != null)
            {
                int? carrierAccountSWVendorAccountId = null;
                HashSet<int> assignRoutesIds = null;
                HashSet<int> alreadyAssignedSWVendorAccountIds = null;
                HashSet<int> supplierIds = null;
                HashSet<int> routeIds = new HashSet<int>();
                if (filter.SupplierIds != null)
                {
                    supplierIds = new HashSet<int>(filter.SupplierIds);
                    foreach (var Id in supplierIds)
                    {
                        var supplierRouteIds = (GetCarrierAccountRouteIds(Id) != null) ? GetCarrierAccountRouteIds(Id) : null;
                        if (supplierRouteIds != null)
                        {
                            foreach (int routeId in supplierRouteIds)
                            { routeIds.Add(routeId); }
                        }

                    }
                }
                if (filter.AssignableToCarrierAccountId.HasValue)
                {
                    var accountManager = new AccountManager();
                    assignRoutesIds = new HashSet<int>(GetCarrierAccountIdsByRouteId().Keys);
                    carrierAccountSWVendorAccountId = accountManager.GetCarrierAccountSWSupplierAccountId(filter.AssignableToCarrierAccountId.Value);
                    alreadyAssignedSWVendorAccountIds = new HashSet<int>(accountManager.GetAllAssignedSWVendorAccountIds());
                }
                filterFunc = (x) =>
                {
                    if (filter.AssignableToCarrierAccountId.HasValue)
                    {
                        if (assignRoutesIds.Contains(x.RouteId))
                            return false;
                        if (carrierAccountSWVendorAccountId.HasValue && x.AccountId != carrierAccountSWVendorAccountId.Value)
                            return false;
                        if (!carrierAccountSWVendorAccountId.HasValue && alreadyAssignedSWVendorAccountIds.Contains(x.AccountId))//if Route belongs to Switch Vendor that is assigned other Carrier Profile
                            return false;
                    }
                    if (routeIds != null)
                    {
                        if (!routeIds.Contains(x.RouteId))
                        {
                            return false;
                        }
                    }
                    return true;
                };
            }
            return allRoutes.MapRecords(RouteEntityInfoMapper, filterFunc);
        }

        public void LinkCarrierAccountToRoutes(int carrierAccountId, List<int> routeIds)
        {
            if (routeIds == null || routeIds.Count == 0)
                throw new ArgumentNullException("routeIds");
            List<Route> routes = new List<Route>();
            int? vendorAccountId = null;
            foreach (var routeId in routeIds)
            {
                var route = GetRoute(routeId);
                route.ThrowIfNull("route", routeId);
                routes.Add(route);
                if (!vendorAccountId.HasValue)
                    vendorAccountId = route.AccountId;
                else if (vendorAccountId.Value != route.AccountId)
                    throw new Exception("All routes should have same AccountId");
            }
            new AccountManager().TrySetSWVendorAccountId(carrierAccountId, vendorAccountId.Value);
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            RouteCarrierAccountExtension routeCarrierAccountExtension = carrierAccountManager.GetExtendedSettings<RouteCarrierAccountExtension>(carrierAccountId);
            if (routeCarrierAccountExtension == null)
                routeCarrierAccountExtension = new RouteCarrierAccountExtension();
            if (routeCarrierAccountExtension.RouteInfo == null)
                routeCarrierAccountExtension.RouteInfo = new List<RouteInfo>();
            foreach (var route in routes)
            {
                routeCarrierAccountExtension.RouteInfo.Add(new RouteInfo { RouteId = route.RouteId });
            }

            carrierAccountManager.UpdateCarrierAccountExtendedSetting<RouteCarrierAccountExtension>(
                carrierAccountId, routeCarrierAccountExtension);
            string carrierAccountName = carrierAccountManager.GetCarrierAccountName(carrierAccountId);
            foreach (var routeId in routeIds)
            {
                GenerateRule(carrierAccountId, routeId, carrierAccountName);
            }
        }
        public List<int> GetCarrierAccountRouteIds(CarrierAccount carrierAccount)
        {
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            RouteCarrierAccountExtension routeExtendedSettings =
                carrierAccountManager.GetExtendedSettings<RouteCarrierAccountExtension>(carrierAccount);
            if (routeExtendedSettings == null) return null;
            List<int> routes = new List<int>();
            if (routeExtendedSettings.RouteInfo != null)
                routes.AddRange(routeExtendedSettings.RouteInfo.Select(itm => itm.RouteId));
            return routes;
        }

        public List<int> GetCarrierAccountRouteIds(int carrierAccountId)
        {
            var carrierAccount = new CarrierAccountManager().GetCarrierAccount(carrierAccountId);
            return carrierAccount != null ? GetCarrierAccountRouteIds(carrierAccount) : null;
        }

        public IDataRetrievalResult<RouteDetail> GetFilteredRoutes(DataRetrievalInput<RouteQuery> input)
        {
            //Get Carrier by id
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();

            RouteCarrierAccountExtension extendedSettingsObject = carrierAccountManager.GetExtendedSettings<RouteCarrierAccountExtension>(input.Query.CarrierAccountId.Value);

            Dictionary<int, RouteInfo> routeInfoDic = new Dictionary<int, RouteInfo>();

            if (extendedSettingsObject != null)
            {
                routeInfoDic = extendedSettingsObject.RouteInfo.ToDictionary(k => k.RouteId, v => v);
            }
            Dictionary<int, Route> routeDic = new Dictionary<int, Route>();
            var allRoutes = this.GetCachedRoutes();

            foreach (var item in routeInfoDic)
            {
                Route route = null;
                if (allRoutes.TryGetValue(item.Key, out route))
                {
                    route.Percentage = item.Value.Percentage;
                    routeDic.Add(route.RouteId, route);
                }
            }
            Func<Route, bool> filterExpression = (x) => (routeDic.ContainsKey(x.RouteId));
            VRActionLogger.Current.LogGetFilteredAction(RouteLoggableEntity.Instance, input);
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, routeDic.ToBigResult(input, filterExpression, RouteDetailMapper));
        }

        public InsertOperationOutput<RouteDetail> AddRoute(RouteToAdd routeItem)
        {
            InsertOperationOutput<RouteDetail> insertOperationOutput = new InsertOperationOutput<RouteDetail>
            {
                Result = InsertOperationResult.Failed,
                InsertedObject = null
            };
            int routeId;
            string mssg;
            if (Insert(routeItem, out routeId, out mssg))
            {
                routeItem.Entity.RouteId = routeId;
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                VRActionLogger.Current.TrackAndLogObjectAdded(RouteLoggableEntity.Instance, routeItem.Entity);
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = RouteDetailMapper(GetRoute(routeId));
            }
            else
            {
                if (!string.IsNullOrEmpty(mssg))
                {
                    insertOperationOutput.Result = InsertOperationResult.Failed;
                    insertOperationOutput.ShowExactMessage = true;
                    insertOperationOutput.Message = mssg;
                }
                else insertOperationOutput.Result = InsertOperationResult.SameExists;
            }
            return insertOperationOutput;
        }

        public int? GetRouteCarrierAccountId(int routeId)
        {
            return GetCarrierAccountIdsByRouteId().GetRecord(routeId);
        }

        public string GetRouteCarrierAccountName(int routeId)
        {
            int? carrierAccountId = GetRouteCarrierAccountId(routeId);
            return carrierAccountId.HasValue ? new CarrierAccountManager().GetCarrierAccountName(carrierAccountId.Value) : null;
        }

        private bool Insert(RouteToAdd routeItem, out int routeId, out string mssg)
        {
            routeId = 0;
            mssg = "";
            var accountManager = new AccountManager();
            var carrierAccountManager = new CarrierAccountManager();
            string carrierName = carrierAccountManager.GetCarrierAccountName(routeItem.CarrierAccountId);
            int? profileId = carrierAccountManager.GetCarrierProfileId(routeItem.CarrierAccountId);

            if (!profileId.HasValue)
                return false;

            int carrierProfileId = (int)profileId;
            var carrierProfileManager = new CarrierProfileManager();
            CarrierProfile carrierProfile = carrierProfileManager.GetCarrierProfile(carrierProfileId);
            AccountCarrierProfileExtension accountExtended = carrierProfileManager.GetExtendedSettings<AccountCarrierProfileExtension>(carrierProfileId);
            int accountId;
            if (accountExtended != null && accountExtended.VendorAccountId.HasValue)
                accountId = accountExtended.VendorAccountId.Value;
            else
            {
                Account account = accountManager.GetAccountInfoFromProfile(carrierProfile, false);
                accountId = accountManager.AddAccount(account);
                AccountCarrierProfileExtension extendedSettings =
                    carrierProfileManager.GetExtendedSettings<AccountCarrierProfileExtension>(carrierProfileId) ?? new AccountCarrierProfileExtension();

                extendedSettings.VendorAccountId = accountId;

                carrierProfileManager.UpdateCarrierProfileExtendedSetting(carrierProfileId, extendedSettings);
            }
            routeItem.Entity.AccountId = accountId;

            RouteCarrierAccountExtension routesExtendedSettings =
                carrierAccountManager.GetExtendedSettings<RouteCarrierAccountExtension>(routeItem.CarrierAccountId) ?? new RouteCarrierAccountExtension();

            var routeInfoList = new List<RouteInfo>();
            if (routesExtendedSettings.RouteInfo != null)
                routeInfoList = routesExtendedSettings.RouteInfo;

            IRouteDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<IRouteDataManager>();
            Helper.SetSwitchConfig(dataManager);
            if (IsPercentageAboveLimit(routeItem.Entity.Percentage, routeInfoList))
            {
                mssg = "Sum of route percentage is above limit";
                return false;
            }
            int? tempRouteId = dataManager.Insert(routeItem.Entity);
            if (tempRouteId.HasValue)
            {
                accountManager.UpdateChannelLimit(routeItem.Entity.AccountId);
                routeId = tempRouteId.Value;
                RouteInfo routeInfo = new RouteInfo
                {
                    RouteId = routeId,
                    Percentage = routeItem.Entity.Percentage
                };
                routeInfoList.Add(routeInfo);
                routesExtendedSettings.RouteInfo = routeInfoList;
                carrierAccountManager.UpdateCarrierAccountExtendedSetting(routeItem.CarrierAccountId, routesExtendedSettings);
                GenerateRule(routeItem.CarrierAccountId, routeId, carrierName);
                return true;
            }
            return false;
        }
        public UpdateOperationOutput<RouteDetail> UpdateRoute(RouteToAdd routeItem)
        {
            var updateOperationOutput = new UpdateOperationOutput<RouteDetail>
            {
                Result = UpdateOperationResult.Failed,
                UpdatedObject = null
            };

            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            RouteCarrierAccountExtension routesExtendedSettings =
                   carrierAccountManager.GetExtendedSettings<RouteCarrierAccountExtension>(routeItem.CarrierAccountId) ??
                   new RouteCarrierAccountExtension();

            List<RouteInfo> routeInfoList = new List<RouteInfo>();
            if (routesExtendedSettings.RouteInfo != null)
                routeInfoList = routesExtendedSettings.RouteInfo;

            if (IsPercentageAboveLimit(routeItem.Entity.Percentage, routeInfoList.Where(r => r.RouteId != routeItem.Entity.RouteId).ToList()))
            {
                updateOperationOutput.Result = UpdateOperationResult.Failed;
                updateOperationOutput.ShowExactMessage = true;
                updateOperationOutput.Message = "Sum of route percentage is above limit";
                return updateOperationOutput;
            }
            IRouteDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<IRouteDataManager>();
            Helper.SetSwitchConfig(dataManager);
            if (dataManager.Update(routeItem.Entity))
            {
                Dictionary<int, RouteInfo> routeInfoDic = routeInfoList.ToDictionary(k => k.RouteId, v => v);

                RouteInfo routeInfo;
                if (routeInfoDic.TryGetValue(routeItem.Entity.RouteId, out routeInfo))
                {
                    if (routeInfo.Percentage != routeItem.Entity.Percentage)
                    {
                        routeInfo.Percentage = routeItem.Entity.Percentage;
                        routesExtendedSettings.RouteInfo = routeInfoList;
                        carrierAccountManager.UpdateCarrierAccountExtendedSetting(routeItem.CarrierAccountId,
                            routesExtendedSettings);
                    }
                    Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                    Route updatedRoute = GetRoute(routeItem.Entity.RouteId);
                    VRActionLogger.Current.TrackAndLogObjectUpdated(RouteLoggableEntity.Instance, updatedRoute);
                    dataManager.UpdateVendorUSer(updatedRoute);
                    updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                    updateOperationOutput.UpdatedObject = RouteDetailMapper(updatedRoute);
                    AccountManager accountManager = new AccountManager();
                    accountManager.UpdateChannelLimit(updatedRoute.AccountId);
                }
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }
            return updateOperationOutput;
        }

        public DateTime GetSwitchDateTime()
        {
            IRouteDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<IRouteDataManager>();
            Helper.SetSwitchConfig(dataManager);
            return dataManager.GetSwitchDateTime();
        }
        #endregion

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IRouteDataManager _dataManager = IVSwitchDataManagerFactory.GetDataManager<IRouteDataManager>();
            protected override bool IsTimeExpirable { get { return true; } }

        }

        private class RouteLoggableEntity : VRLoggableEntityBase
        {
            public static RouteLoggableEntity Instance = new RouteLoggableEntity();

            private RouteLoggableEntity()
            {

            }

            static RouteManager routeManager = new RouteManager();

            public override string EntityUniqueName
            {
                get { return "NP_IVSwitch_Route"; }
            }

            public override string ModuleName
            {
                get { return "IVSwitch"; }
            }

            public override string EntityDisplayName
            {
                get { return "Route"; }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "NP_IVSwitch_Route_ViewHistoryItem"; }
            }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                Route route = context.Object.CastWithValidate<Route>("context.Object");
                return route.RouteId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                Route route = context.Object.CastWithValidate<Route>("context.Object");
                return routeManager.GetRouteDescription(route.RouteId);
            }
        }

        #endregion

        #region Private Methods

        private bool IsPercentageAboveLimit(int percentage, List<RouteInfo> routeInfos)
        {
            int percentageSum = routeInfos.Sum(r => r.Percentage);
            return percentageSum + percentage > 100;
        }
        private void GenerateRule(int carrierId, int routeId, string carrierName)
        {
            MappingRuleHelper mappingRuleHelper = new MappingRuleHelper(carrierId, routeId, 2, carrierName);
            mappingRuleHelper.BuildRule();
        }
        Dictionary<int, Route> GetCachedRoutes()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetRoute",
                PrepareCachedRoutes);
        }

        private Dictionary<int, Route> PrepareCachedRoutes()
        {
            IRouteDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<IRouteDataManager>();
            Helper.SetSwitchConfig(dataManager);
            Dictionary<int, Route> routes = new Dictionary<int, Route>();

            if (dataManager.IvSwitchSync == null) return routes;

            routes = dataManager.GetRoutes().ToDictionary(x => x.RouteId, x => x);
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            var suppliers = carrierAccountManager.GetAllSuppliers();

            foreach (var supplierItem in suppliers)
            {
                RouteCarrierAccountExtension extendedSettingsObject = carrierAccountManager.GetExtendedSettings<RouteCarrierAccountExtension>(supplierItem);
                if (extendedSettingsObject == null) continue;
                foreach (var routeInfo in extendedSettingsObject.RouteInfo)
                {
                    Route tempRoute;
                    if (routes.TryGetValue(routeInfo.RouteId, out tempRoute))
                    {
                        tempRoute.Percentage = routeInfo.Percentage;
                    }
                }

            }
            return routes;
        }
        public Dictionary<int, int> GetCarrierAccountIdsByRouteId()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<TOne.WhS.BusinessEntity.Business.CarrierAccountManager.CacheManager>().GetOrCreateObject("IVSwitch_GetCarrierAccountIdsByRouteId",
                () =>
                {
                    Dictionary<int, int> mappeDictionary = new Dictionary<int, int>();
                    CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                    var suppliers = carrierAccountManager.GetAllSuppliers();

                    foreach (var supplierItem in suppliers)
                    {
                        RouteCarrierAccountExtension extendedSettingsObject = carrierAccountManager.GetExtendedSettings<RouteCarrierAccountExtension>(supplierItem);
                        if (extendedSettingsObject == null) continue;
                        foreach (var routeInfo in extendedSettingsObject.RouteInfo)
                        {
                            if (!mappeDictionary.ContainsKey(routeInfo.RouteId))
                            {
                                mappeDictionary[routeInfo.RouteId] = supplierItem.CarrierAccountId;
                            }
                        }

                    }
                    return mappeDictionary;
                });
        }
        #endregion

        #region Mappers

        public RouteDetail RouteDetailMapper(Route route)
        {
            RouteDetail routeDetail = new RouteDetail()
            {
                Entity = route,
                CurrentStateDescription = Vanrise.Common.Utilities.GetEnumDescription<State>(route.CurrentState),

            };

            return routeDetail;
        }

        public RouteEntityInfo RouteEntityInfoMapper(Route route)
        {

            RouteEntityInfo routeInfo = new RouteEntityInfo()
            {
                RouteId = route.RouteId,
                Description = GetRouteDescription(route),
                AccountId = route.AccountId

            };

            return routeInfo;

        }
        #endregion
    }
}

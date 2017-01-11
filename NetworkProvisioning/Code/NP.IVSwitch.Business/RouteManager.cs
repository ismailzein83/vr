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

namespace NP.IVSwitch.Business
{
    public class RouteManager
    {
        #region Public Methods
        public Route GetRoute(int routeId)
        {
            Dictionary<int, Route> cachedRoute = this.GetCachedRoutes();
            return cachedRoute.GetRecord(routeId);
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
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
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

        private bool Insert(RouteToAdd routeItem, out int routeId, out string mssg)
        {
            routeId = 0;
            mssg = "";
            AccountManager accountManager = new AccountManager();
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            string carrierName = carrierAccountManager.GetCarrierAccountName(routeItem.CarrierAccountId);
            var profileId = carrierAccountManager.GetCarrierProfileId(routeItem.CarrierAccountId);

            if (!profileId.HasValue) return false;

            int carrierProfileId = (int)profileId;
            CarrierProfileManager carrierProfileManager = new CarrierProfileManager();
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
                    carrierProfileManager.GetExtendedSettings<AccountCarrierProfileExtension>(carrierProfileId) ??
                    new AccountCarrierProfileExtension();

                extendedSettings.VendorAccountId = accountId;

                carrierProfileManager.UpdateCarrierProfileExtendedSetting(carrierProfileId, extendedSettings);
            }
            routeItem.Entity.AccountId = accountId;

            RouteCarrierAccountExtension routesExtendedSettings =
                carrierAccountManager.GetExtendedSettings<RouteCarrierAccountExtension>(
                    routeItem.CarrierAccountId) ??
                new RouteCarrierAccountExtension();

            List<RouteInfo> routeInfoList = new List<RouteInfo>();
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
                carrierAccountManager.UpdateCarrierAccountExtendedSetting(routeItem.CarrierAccountId,
                    routesExtendedSettings);
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
            Dictionary<int, Route> routes = dataManager.GetRoutes().ToDictionary(x => x.RouteId, x => x);
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
        public Dictionary<int, int> GetRouteAndSupplierIds()
        {
            Dictionary<int, int> mappeDictionary = new Dictionary<int, int>();
            IRouteDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<IRouteDataManager>();
            Helper.SetSwitchConfig(dataManager);
            Dictionary<int, Route> routes = dataManager.GetRoutes().ToDictionary(x => x.RouteId, x => x);
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
                        mappeDictionary[routeInfo.RouteId] = supplierItem.CarrierAccountId;
                    }
                }

            }
            return mappeDictionary;
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

        #endregion
    }
}

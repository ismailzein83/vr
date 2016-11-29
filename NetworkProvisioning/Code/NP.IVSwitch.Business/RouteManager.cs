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
    public  class RouteManager
    {
        #region Public Methods
        public Route GetRoute(int routeId)
        {
            Dictionary<int, Route> cachedRoute = this.GetCachedRoute();
            return cachedRoute.GetRecord(routeId);
        }

        public IDataRetrievalResult<RouteDetail> GetFilteredRoutes(DataRetrievalInput<RouteQuery> input)
        {
            //Get Carrier by id
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
 
            RouteCarrierAccountExtension extendedSettingsObject = carrierAccountManager.GetExtendedSettingsObject<RouteCarrierAccountExtension>(input.Query.CarrierAccountId.Value);

            List<int> routeIdList = new List<int>();

            if (extendedSettingsObject != null)
            {
                routeIdList = extendedSettingsObject.RouteIds;
            }

            var allRoutes = this.GetCachedRoute(); 
            Func<Route, bool> filterExpression = (x) =>  (routeIdList.Contains(x.RouteId));
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allRoutes.ToBigResult(input, filterExpression, RouteDetailMapper));
        }

        public Vanrise.Entities.InsertOperationOutput<RouteDetail> AddRoute(RouteToAdd routeItem)
        {

            int carrierAccountId = routeItem.CarrierAccountId;

            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            int carrierProfileId = (int)carrierAccountManager.GetCarrierProfileId(carrierAccountId); // Get CarrierProfileId

            CarrierProfileManager carrierProfileManager = new CarrierProfileManager();
            CarrierProfile carrierProfile = carrierProfileManager.GetCarrierProfile(carrierProfileId); // Get CarrierProfile


            AccountCarrierProfileExtension accountExtended = carrierProfileManager.GetExtendedSettingsObject<AccountCarrierProfileExtension>(carrierProfileId);
            int accountId = -1;

 
             if (accountExtended != null && accountExtended.VendorAccountId.HasValue)
             {
                 accountId = accountExtended.VendorAccountId.Value;
             }
             else
             {
                              
                //create the account
                AccountManager accountManager = new AccountManager();
                Account account = accountManager.GetAccountInfoFromProfile(carrierProfile, false);
                accountId = accountManager.AddAccount(account);
 
                // add it to extendedsettings
                AccountCarrierProfileExtension extendedSettings = new AccountCarrierProfileExtension();
                AccountCarrierProfileExtension ExtendedSettingsObject = carrierProfileManager.GetExtendedSettingsObject<AccountCarrierProfileExtension>(carrierProfileId);
                if (ExtendedSettingsObject != null)
                    extendedSettings = (AccountCarrierProfileExtension)ExtendedSettingsObject;

                extendedSettings.VendorAccountId = accountId;

                carrierProfileManager.UpdateCarrierProfileExtendedSetting(carrierProfileId, extendedSettings);
            }

            routeItem.Entity.AccountId = accountId;
 

            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<RouteDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            IRouteDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<IRouteDataManager>();

            int routeId = -1;


            if (dataManager.Insert(routeItem.Entity, out  routeId))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = RouteDetailMapper(this.GetRoute(routeId));


                RouteCarrierAccountExtension routesExtendedSettings = carrierAccountManager.GetExtendedSettingsObject<RouteCarrierAccountExtension>(carrierAccountId);
                //add route to carrier account
                if (routesExtendedSettings == null)
                    routesExtendedSettings = new RouteCarrierAccountExtension();

                List<int> routeIds = new List<int>();
                if (routesExtendedSettings.RouteIds != null)
                    routeIds = routesExtendedSettings.RouteIds;

                // tariffs
                dataManager.CheckTariffTable();                 
                 

                routeIds.Add(routeId);
                routesExtendedSettings.RouteIds = routeIds;

                carrierAccountManager.UpdateCarrierAccountExtendedSetting(carrierAccountId, routesExtendedSettings);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<RouteDetail> UpdateRoute(Route routeItem)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<RouteDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IRouteDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<IRouteDataManager>();


            if (dataManager.Update(routeItem))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = RouteDetailMapper(this.GetRoute(routeItem.RouteId));
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
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

        Dictionary<int, Route> GetCachedRoute()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetRoute",
                () =>
                {
                    IRouteDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<IRouteDataManager>();
                    return dataManager.GetRoutes().ToDictionary(x => x.RouteId, x => x);
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

        #endregion
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Caching;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SwitchConnectivityManager
    {
        #region Fields

        CarrierAccountManager _carrierAccountManager = new CarrierAccountManager();
        SwitchManager _switchManager = new SwitchManager();
        
        #endregion

        #region Public Methods

        public Vanrise.Entities.IDataRetrievalResult<SwitchConnectivityDetail> GetFilteredSwitchConnectivities(Vanrise.Entities.DataRetrievalInput<SwitchConnectivityQuery> input)
        {
            Dictionary<int, SwitchConnectivity> cachedEntities = this.GetCachedSwitchConnectivities();

            Func<SwitchConnectivity, bool> filterExpression = (itm) =>
                (input.Query.Name == null || itm.Name.ToLower().Contains(input.Query.Name.ToLower())) &&
                (input.Query.CarrierAccountIds == null || input.Query.CarrierAccountIds.Contains(itm.CarrierAccountId)) &&
                (input.Query.SwitchIds == null || input.Query.SwitchIds.Contains(itm.SwitchId)) &&
                (input.Query.ConnectionTypes == null || input.Query.ConnectionTypes.Contains(itm.Settings.ConnectionType));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, cachedEntities.ToBigResult(input, filterExpression, SwitchConnectivityDetailMapper));
        }

        public SwitchConnectivity GetSwitchConnectivity(int switchConnectivityId)
        {
            Dictionary<int, SwitchConnectivity> cachedEntities = this.GetCachedSwitchConnectivities();
            return cachedEntities.GetRecord(switchConnectivityId);
        }

        public Vanrise.Entities.InsertOperationOutput<SwitchConnectivityDetail> AddSwitchConnectivity(SwitchConnectivity switchConnectivity)
        {
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<SwitchConnectivityDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            ISwitchConnectivityDataManager dataManager = BEDataManagerFactory.GetDataManager<ISwitchConnectivityDataManager>();
            int insertedId = -1;

            if (dataManager.Insert(switchConnectivity, out insertedId))
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                switchConnectivity.SwitchConnectivityId = insertedId;
                insertOperationOutput.InsertedObject = SwitchConnectivityDetailMapper(switchConnectivity);
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<SwitchConnectivityDetail> UpdateSwitchConnectivity(SwitchConnectivity switchConnectivity)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<SwitchConnectivityDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            ISwitchConnectivityDataManager dataManager = BEDataManagerFactory.GetDataManager<ISwitchConnectivityDataManager>();

            if (dataManager.Update(switchConnectivity))
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = SwitchConnectivityDetailMapper(switchConnectivity);
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public SwitchConnectivity GetMatchConnectivity(string port)
        {
            Dictionary<string, SwitchConnectivity> switchConnectivityByPort = GetSwitchConnectivitiesByPort();
            if (switchConnectivityByPort == null)
                return null;
            SwitchConnectivity connectivity;
            if (switchConnectivityByPort.TryGetValue(port, out connectivity))
                return connectivity;
            else
                return null;
        }

        public string GetMatchConnectivityName(string port)
        {
            var connectivity = GetMatchConnectivity(port);
            return connectivity != null ? connectivity.Name : null;
        }

        private Dictionary<string, SwitchConnectivity> GetSwitchConnectivitiesByPort()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetSwitchConnectivitiesByPort", () =>
            {
                Dictionary<string, SwitchConnectivity> switchConnectivitiesByPort = new Dictionary<string, SwitchConnectivity>();
                var switchConnectivitiesById = GetCachedSwitchConnectivities();
                if (switchConnectivitiesById != null)
                {
                    foreach(var switchConnectivity in switchConnectivitiesById.Values)
                    {
                        if(switchConnectivity.Settings != null && switchConnectivity.Settings.Trunks != null)
                        {
                            foreach(var trunk in switchConnectivity.Settings.Trunks)
                            {
                                if (!switchConnectivitiesByPort.ContainsKey(trunk.Name))
                                    switchConnectivitiesByPort.Add(trunk.Name, switchConnectivity);
                            }
                        }
                    }
                }
                return switchConnectivitiesByPort;
            });
        }

        #endregion

        #region Private Classes

        internal class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ISwitchConnectivityDataManager _dataManager = BEDataManagerFactory.GetDataManager<ISwitchConnectivityDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreSwitchConnectivitiesUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region Private Methods

        Dictionary<int, SwitchConnectivity> GetCachedSwitchConnectivities()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetSwitchConnectivities", () =>
            {
                ISwitchConnectivityDataManager dataManager = BEDataManagerFactory.GetDataManager<ISwitchConnectivityDataManager>();
                IEnumerable<SwitchConnectivity> switchConnectivities = dataManager.GetSwitchConnectivities();
                return switchConnectivities.ToDictionary(kvp => kvp.SwitchConnectivityId, kvp => kvp);
            });
        }

        #endregion

        #region Mappers

        SwitchConnectivityDetail SwitchConnectivityDetailMapper(SwitchConnectivity switchConnectivity)
        {
            return new SwitchConnectivityDetail()
            {
                Entity = switchConnectivity,
                CarrierAccountName = _carrierAccountManager.GetCarrierAccountName(switchConnectivity.CarrierAccountId),
                SwitchName = _switchManager.GetSwitchName(switchConnectivity.SwitchId)
            };
        }

        #endregion
    }
}

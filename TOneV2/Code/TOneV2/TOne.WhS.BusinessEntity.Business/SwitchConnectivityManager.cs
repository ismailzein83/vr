using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SwitchConnectivityManager : IBusinessEntityManager
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

        public string GetSwitchConnectivityName(int switchConnectivityId)
        {
            SwitchConnectivity switchConnectivity = GetSwitchConnectivity(switchConnectivityId);
            if (switchConnectivity == null)
                return null;
            return switchConnectivity.Name;
        }

        public IEnumerable<SwitchConnectivityInfo> GetSwitcheConnectivitiesInfo()
        {
            return GetCachedSwitchConnectivities().MapRecords(SwitchConnectivityInfoMapper).OrderBy(x => x.Name);
        }

        public Vanrise.Entities.InsertOperationOutput<SwitchConnectivityDetail> AddSwitchConnectivity(SwitchConnectivity switchConnectivity)
        {
            ValidateSwitchConnectivityToAdd(switchConnectivity);

            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<SwitchConnectivityDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            ISwitchConnectivityDataManager dataManager = BEDataManagerFactory.GetDataManager<ISwitchConnectivityDataManager>();
            int insertedId = -1;

            if (dataManager.Insert(switchConnectivity, out insertedId))
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                switchConnectivity.SwitchConnectivityId = insertedId;
                insertOperationOutput.InsertedObject = SwitchConnectivityDetailMapper(switchConnectivity);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<SwitchConnectivityDetail> UpdateSwitchConnectivity(SwitchConnectivityToEdit switchConnectivity)
        {
            ValidateSwitchConnectivityToEdit(switchConnectivity);

            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<SwitchConnectivityDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            ISwitchConnectivityDataManager dataManager = BEDataManagerFactory.GetDataManager<ISwitchConnectivityDataManager>();

            if (dataManager.Update(switchConnectivity))
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = SwitchConnectivityDetailMapper(this.GetSwitchConnectivity(switchConnectivity.SwitchConnectivityId));
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
                    foreach (var switchConnectivity in switchConnectivitiesById.Values)
                    {
                        if (switchConnectivity.Settings != null && switchConnectivity.Settings.Trunks != null)
                        {
                            foreach (var trunk in switchConnectivity.Settings.Trunks)
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

        public dynamic GetEntity(IBusinessEntityGetByIdContext context)
        {
            return GetSwitchConnectivity(context.EntityId);
        }

        public List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context)
        {
            var switchConnectivities = GetCachedSwitchConnectivities();
            if (switchConnectivities == null)
                return null;
            else
                return switchConnectivities.Select(itm => itm as dynamic).ToList();
        }

        public bool IsCacheExpired(IBusinessEntityIsCacheExpiredContext context, ref DateTime? lastCheckTime)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().IsCacheExpired(ref lastCheckTime);
        }

        public string GetEntityDescription(IBusinessEntityDescriptionContext context)
        {
            return GetSwitchConnectivityName(Convert.ToInt32(context.EntityId));
        }

        #endregion

        #region Validation Methods

        void ValidateSwitchConnectivityToAdd(SwitchConnectivity switchConnectivity)
        {
            ValidateSwitchConnectivity(switchConnectivity.Name, switchConnectivity.CarrierAccountId, switchConnectivity.SwitchId, switchConnectivity.Settings, switchConnectivity.BED);
        }

        void ValidateSwitchConnectivityToEdit(SwitchConnectivityToEdit switchConnectivity)
        {
            ValidateSwitchConnectivity(switchConnectivity.Name, switchConnectivity.CarrierAccountId, switchConnectivity.SwitchId, switchConnectivity.Settings, switchConnectivity.BED);
        }

        void ValidateSwitchConnectivity(string scName, int scCarrierAccountId, int scSwitchId, SwitchConnectivitySettings scSettings, DateTime scBED)
        {
            if (String.IsNullOrWhiteSpace(scName))
                throw new MissingArgumentValidationException("SwitchConnectivity.Name");

            var carrierAccountManager = new CarrierAccountManager();
            var carrierAccount = carrierAccountManager.GetCarrierAccount(scCarrierAccountId);
            if (carrierAccount == null)
                throw new DataIntegrityValidationException(String.Format("CarrierAccount '{0}' does not exist", scCarrierAccountId));

            var switchManager = new SwitchManager();
            var whsSwitch = switchManager.GetSwitch(scSwitchId);
            if (whsSwitch == null)
                throw new DataIntegrityValidationException(String.Format("Switch '{0}' does not exist", scSwitchId));

            if (scSettings == null)
                throw new MissingArgumentValidationException("SwitchConnectivity.Settings");

            if (scSettings.Trunks == null || scSettings.Trunks.Count == 0)
                throw new MissingArgumentValidationException("SwitchConnectivity.Settings.Trunks");

            var existingTrunkNames = new List<string>();
            for (int i = 0; i < scSettings.Trunks.Count; i++)
            {
                SwitchConnectivityTrunk trunk = scSettings.Trunks[i];
                if (trunk == null || String.IsNullOrWhiteSpace(trunk.Name))
                    throw new MissingArgumentValidationException(String.Format("SwitchConnectivityTrunk '{0}'", (i + 1)));
                string trunkName = trunk.Name.ToLower();
                string existingTrunkName = existingTrunkNames.FindRecord(itm => itm == trunkName);
                if (existingTrunkName != null)
                    throw new DataIntegrityValidationException(String.Format("Trunk '{0}' exists multiple times", trunk.Name));
                existingTrunkNames.Add(trunkName);
            }

            if (scBED == default(DateTime))
                throw new MissingArgumentValidationException("SwitchConnectivity.BED");
        }

        #endregion

        #region Private Classes

        internal class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            DateTime? _carrierAccountLastCheck;

            ISwitchConnectivityDataManager _dataManager = BEDataManagerFactory.GetDataManager<ISwitchConnectivityDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreSwitchConnectivitiesUpdated(ref _updateHandle)
                    | Vanrise.Caching.CacheManagerFactory.GetCacheManager<CarrierAccountManager.CacheManager>().IsCacheExpired(ref _carrierAccountLastCheck);
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
                Dictionary<int, SwitchConnectivity> dic = new Dictionary<int, SwitchConnectivity>();
                CarrierAccountManager carrierAccountManager = new CarrierAccountManager();

                foreach (SwitchConnectivity item in switchConnectivities)
                {
                    if (!carrierAccountManager.IsCarrierAccountDeleted(item.CarrierAccountId))
                        dic.Add(item.SwitchConnectivityId, item);
                }

                return dic;
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
        private SwitchConnectivityInfo SwitchConnectivityInfoMapper(SwitchConnectivity switchConnectivity)
        {
            return new SwitchConnectivityInfo()
            {
                SwitchConnectivityId = switchConnectivity.SwitchConnectivityId,
                Name = switchConnectivity.Name,
            };
        }
        #endregion


        public IEnumerable<dynamic> GetIdsByParentEntityId(IBusinessEntityGetIdsByParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public dynamic GetParentEntityId(IBusinessEntityGetParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }


        public dynamic MapEntityToInfo(IBusinessEntityMapToInfoContext context)
        {
            throw new NotImplementedException();
        }
    }
}

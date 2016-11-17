using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SwitchManager : IBusinessEntityManager
    {
        #region Public Methods

        public Vanrise.Entities.IDataRetrievalResult<SwitchDetail> GetFilteredSwitches(Vanrise.Entities.DataRetrievalInput<SwitchQuery> input)
        {
            var allSwitches = GetCachedSwitches();

            Func<Switch, bool> filterExpression = (prod) =>
                 (input.Query.Name == null || prod.Name.ToLower().Contains(input.Query.Name.ToLower()));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allSwitches.ToBigResult(input, filterExpression, SwitchDetailMapper));
        }

        public Switch GetSwitch(int switchId)
        {
            var switchs = GetCachedSwitches();
            return switchs.GetRecord(switchId);
        }

        public List<Switch> GetAllSwitches()
        {
            var cachedSwitches = GetCachedSwitches();
            return cachedSwitches != null ? cachedSwitches.Values.ToList() : null;
        }

        public IEnumerable<SwitchInfo> GetSwitchesInfo()
        {
            return GetCachedSwitches().MapRecords(SwitchInfoMapper).OrderBy(x => x.Name);
        }

        public string GetSwitchName(int switchId)
        {
            Switch switchObj = GetSwitch(switchId);

            if (switchObj != null)
                return switchObj.Name;

            return null;
        }

        public dynamic GetEntity(IBusinessEntityGetByIdContext context)
        {
            return GetSwitch(context.EntityId);
        }

        public List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context)
        {
            var switchConnectivities = GetCachedSwitches();
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
            return GetSwitchName(Convert.ToInt32(context.EntityId));
        }

        public Vanrise.Entities.InsertOperationOutput<SwitchDetail> AddSwitch(Switch whsSwitch)
        {
            ValidateSwitchToAdd(whsSwitch);

            Vanrise.Entities.InsertOperationOutput<SwitchDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<SwitchDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            int switchId = -1;

            ISwitchDataManager dataManager = BEDataManagerFactory.GetDataManager<ISwitchDataManager>();
            bool insertActionSucc = dataManager.Insert(whsSwitch, out switchId);
            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                whsSwitch.SwitchId = switchId;
                insertOperationOutput.InsertedObject = SwitchDetailMapper(whsSwitch);
            }
            else
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<SwitchDetail> UpdateSwitch(SwitchToEdit whsSwitch)
        {
            ValidateSwitchToUpdate(whsSwitch);

            ISwitchDataManager dataManager = BEDataManagerFactory.GetDataManager<ISwitchDataManager>();

            bool updateActionSucc = dataManager.Update(whsSwitch);
            Vanrise.Entities.UpdateOperationOutput<SwitchDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<SwitchDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = SwitchDetailMapper(this.GetSwitch(whsSwitch.SwitchId));
            }
            else
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            return updateOperationOutput;
        }

        public Vanrise.Entities.DeleteOperationOutput<SwitchDetail> DeleteSwitch(int switchId)
        {
            ISwitchDataManager dataManager = BEDataManagerFactory.GetDataManager<ISwitchDataManager>();
            Vanrise.Entities.DeleteOperationOutput<SwitchDetail> deleteOperationOutput = new Vanrise.Entities.DeleteOperationOutput<SwitchDetail>();
            bool updateActionSucc = dataManager.Delete(switchId);
            if (updateActionSucc)
            {
                deleteOperationOutput.Result = Vanrise.Entities.DeleteOperationResult.Succeeded;
            }
            else
                deleteOperationOutput.Result = Vanrise.Entities.DeleteOperationResult.Failed;
            return deleteOperationOutput;
        }

        public SwitchCDPNsForIdentification GetSwitchCDPNsForIdentification(int switchId, string inputCDPN, string cdpnIn, string cdpnOut)
        {
            Dictionary<SwitchCDPN, CDPNIdentification> mappingResults = GetCachedMappingSwitchCDPNs(switchId);

            string customerCDPN = GetCDPNPropertyValue(mappingResults.GetRecord(SwitchCDPN.CustomerCDPN), inputCDPN, cdpnIn, cdpnOut);
            string supplierCDPN = GetCDPNPropertyValue(mappingResults.GetRecord(SwitchCDPN.SupplierCDPN), inputCDPN, cdpnIn, cdpnOut);
            string outputCDPN = GetCDPNPropertyValue(mappingResults.GetRecord(SwitchCDPN.CDPN), inputCDPN, cdpnIn, cdpnOut);

            return new SwitchCDPNsForIdentification
            {
                CustomerCDPN = customerCDPN,
                SupplierCDPN = supplierCDPN,
                OutputCDPN = outputCDPN,
            };
        }

        public SwitchCDPNsForZoneMatch GetSwitchCDPNsForZoneMatch(int switchId, string inputCDPN, string cdpnIn, string cdpnOut)
        {
            Dictionary<SwitchCDPN, CDPNIdentification> mappingResults = GetCachedMappingSwitchCDPNs(switchId);

            string saleZoneCDPN = GetCDPNPropertyValue(mappingResults.GetRecord(SwitchCDPN.SaleZoneCDPN), inputCDPN, cdpnIn, cdpnOut);
            string supplierZoneCDPN = GetCDPNPropertyValue(mappingResults.GetRecord(SwitchCDPN.SupplierZoneCDPN), inputCDPN, cdpnIn, cdpnOut);

            return new SwitchCDPNsForZoneMatch
            {
                SaleZoneCDPN = saleZoneCDPN,
                SupplierZoneCDPN = supplierZoneCDPN,
            };
        }

        private Dictionary<SwitchCDPN, CDPNIdentification> GetCachedMappingSwitchCDPNs(int switchId)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetMappingSwitchCDPNs",
                () =>
                {
                    ConfigManager configManager = new ConfigManager();
                    Switch currentSwitch = this.GetSwitch(switchId);
                    if (currentSwitch == null)
                        throw new NullReferenceException(string.Format("currentSwitch for ID: {0}", switchId));

                    SwitchCDRMappingConfiguration switchCDRMappingConfiguration = currentSwitch.Settings != null ? currentSwitch.Settings.SwitchCDRMappingConfiguration : null;

                    Dictionary<SwitchCDPN, CDPNIdentification> mappingResults = new Dictionary<SwitchCDPN, CDPNIdentification>();
                    if (switchCDRMappingConfiguration != null)
                    {
                        mappingResults.Add(SwitchCDPN.CDPN, _GetCorrespondingCDPNIdentification(switchCDRMappingConfiguration.GeneralIdentification, configManager.GetGeneralCDPNIndentification()));
                        mappingResults.Add(SwitchCDPN.CustomerCDPN, _GetCorrespondingCDPNIdentification(switchCDRMappingConfiguration.CustomerIdentification, configManager.GetCustomerCDPNIndentification()));
                        mappingResults.Add(SwitchCDPN.SupplierCDPN, _GetCorrespondingCDPNIdentification(switchCDRMappingConfiguration.SupplierIdentification, configManager.GetSupplierCDPNIndentification()));
                        mappingResults.Add(SwitchCDPN.SaleZoneCDPN, _GetCorrespondingCDPNIdentification(switchCDRMappingConfiguration.SaleZoneIdentification, configManager.GetSaleZoneCDPNIndentification()));
                        mappingResults.Add(SwitchCDPN.SupplierZoneCDPN, _GetCorrespondingCDPNIdentification(switchCDRMappingConfiguration.SupplierZoneIdentification, configManager.GetSupplierZoneCDPNIndentification()));
                    }
                    else
                    {
                        mappingResults.Add(SwitchCDPN.CDPN, configManager.GetGeneralCDPNIndentification());
                        mappingResults.Add(SwitchCDPN.CustomerCDPN, configManager.GetCustomerCDPNIndentification());
                        mappingResults.Add(SwitchCDPN.SupplierCDPN, configManager.GetSupplierCDPNIndentification());
                        mappingResults.Add(SwitchCDPN.SaleZoneCDPN, configManager.GetSaleZoneCDPNIndentification());
                        mappingResults.Add(SwitchCDPN.SupplierZoneCDPN, configManager.GetSupplierZoneCDPNIndentification());
                    }
                    return mappingResults;
                });
        }
        #endregion

        #region Validation Methods

        void ValidateSwitchToAdd(Switch whsSwitch)
        {
            ValidateSwitch(whsSwitch.Name);
        }

        void ValidateSwitchToUpdate(SwitchToEdit whsSwitch)
        {
            ValidateSwitch(whsSwitch.Name);
        }

        void ValidateSwitch(string sName)
        {
            if (String.IsNullOrWhiteSpace(sName))
                throw new MissingArgumentValidationException("Switch.Name");
        }

        #endregion

        #region Private Classes

        class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            DateTime? _SettingsCacheLastCheck;

            ISwitchDataManager _dataManager = BEDataManagerFactory.GetDataManager<ISwitchDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreSwitchesUpdated(ref _updateHandle)
                    |
                    Vanrise.Caching.CacheManagerFactory.GetCacheManager<SettingManager.CacheManager>().IsCacheExpired(ref _SettingsCacheLastCheck);
            }
        }

        #endregion

        #region Private Methods

        Dictionary<int, Switch> GetCachedSwitches()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetSwitches",
               () =>
               {
                   ISwitchDataManager dataManager = BEDataManagerFactory.GetDataManager<ISwitchDataManager>();
                   IEnumerable<Switch> switchs = dataManager.GetSwitches();
                   return switchs.ToDictionary(cn => cn.SwitchId, cn => cn);
               });
        }

        private CDPNIdentification _GetCorrespondingCDPNIdentification(CDPNIdentification? switchCDPNIdentification, CDPNIdentification defaultCDPNIdentification)
        {
            if (switchCDPNIdentification != null && switchCDPNIdentification.HasValue)
                return switchCDPNIdentification.Value;

            return defaultCDPNIdentification;
        }

        private string GetCDPNPropertyValue(CDPNIdentification cdpnIdentification, string inputCDPN, string cdpnIn, string cdpnOut)
        {
            switch (cdpnIdentification)
            {
                case CDPNIdentification.CDPN: return inputCDPN;
                case CDPNIdentification.CDPNIn: return cdpnIn;
                case CDPNIdentification.CDPNOut: return cdpnOut;
                default: throw new NotSupportedException("cdpnIdentification");
            }
        }
        #endregion

        #region Mappers

        private SwitchInfo SwitchInfoMapper(Switch whsSwitch)
        {
            return new SwitchInfo()
            {
                SwitchId = whsSwitch.SwitchId,
                Name = whsSwitch.Name,
            };
        }

        private SwitchDetail SwitchDetailMapper(Switch whsSwitch)
        {
            SwitchDetail switchDetail = new SwitchDetail();
            switchDetail.Entity = whsSwitch;
            return switchDetail;
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
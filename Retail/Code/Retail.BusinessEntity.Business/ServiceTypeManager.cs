using Retail.BusinessEntity.Data;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
namespace Retail.BusinessEntity.Business
{
    public class ServiceTypeManager
    {
        #region Public Methods

        public Vanrise.Entities.IDataRetrievalResult<ServiceTypeDetail> GetFilteredServiceTypes(Vanrise.Entities.DataRetrievalInput<ServiceTypeQuery> input)
        {
            Dictionary<int, ServiceType> cachedServiceTypes = this.GetCachedServiceTypes();

            Func<ServiceType, bool> filterExpression = (serviceType) =>
                (input.Query.Name == null || serviceType.Name.ToLower().Contains(input.Query.Name.ToLower()));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, cachedServiceTypes.ToBigResult(input, filterExpression, ServiceTypeDetailMapper));
        }

        public IEnumerable<ServiceTypeInfo> GetServiceTypesInfo()
        {
            return this.GetCachedServiceTypes().MapRecords(ServiceTypeInfoMapper).OrderBy(x => x.Name);
        }

        public ServiceType GetServiceType(int serviceTypeId)
        {
            Dictionary<int, ServiceType> cachedServiceTypes = this.GetCachedServiceTypes();
            return cachedServiceTypes.GetRecord(serviceTypeId);
        }

        public string GetServiceTypeName(int serviceTypeId)
        {
            ServiceType serviceType = this.GetServiceType(serviceTypeId);
            return (serviceType != null) ? serviceType.Name : null;
        }

        public ChargingPolicyDefinitionSettings GetServiceTypeChargingPolicyDefinitionSettings(int serviceTypeId)
        {
            ServiceType serviceType = this.GetServiceType(serviceTypeId);
            return (serviceType != null && serviceType.Settings != null) ? serviceType.Settings.ChargingPolicyDefinitionSettings : null;
        }

        public Vanrise.Entities.UpdateOperationOutput<ServiceTypeDetail> UpdateServiceType(ServiceTypeToEdit updatedServiceType)
        {
            ValidateServiceTypeToEdit(updatedServiceType);

            ServiceType serviceType = GetServiceType(updatedServiceType.ServiceTypeId);
            ServiceTypeSettings serviceTypeSettings = new ServiceTypeSettings
            {
                AccountServiceEditor = serviceType.Settings.AccountServiceEditor,
                ServiceVolumeEditor = serviceType.Settings.ServiceVolumeEditor,
                ChargingPolicyDefinitionSettings = updatedServiceType.ChargingPolicyDefinitionSettings,
                Description = updatedServiceType.Description,
                IdentificationRuleDefinitionId = updatedServiceType.IdentificationRuleDefinitionId
            };
            if (serviceType.Settings.ChargingPolicyDefinitionSettings !=null)
              serviceTypeSettings.ChargingPolicyDefinitionSettings.ChargingPolicyEditor = serviceType.Settings.ChargingPolicyDefinitionSettings.ChargingPolicyEditor;
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<ServiceTypeDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IServiceTypeDataManager dataManager = BEDataManagerFactory.GetDataManager<IServiceTypeDataManager>();

            if (dataManager.Update(updatedServiceType.ServiceTypeId, updatedServiceType.Title, serviceTypeSettings))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = ServiceTypeDetailMapper(this.GetServiceType(serviceType.ServiceTypeId));
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public IEnumerable<ChargingPolicyDefinitionConfig> GetChargingPolicyTemplateConfigs()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<ChargingPolicyDefinitionConfig>(ChargingPolicyDefinitionConfig.EXTENSION_TYPE);
        }
        public IEnumerable<ChargingPolicyPartTypeConfig> GetChargingPolicyPartTypeTemplateConfigs()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<ChargingPolicyPartTypeConfig>(ChargingPolicyPartTypeConfig.EXTENSION_TYPE);
        }

        public IEnumerable<ChargingPolicyPartConfig> GetChargingPolicyPartTemplateConfigs(int partTypeConfigId)
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            var partType = GetChargingPolicyPartTypeTemplateConfigs().FindRecord(itm => itm.ExtensionConfigurationId == partTypeConfigId);
            if (partType == null)
                throw new NullReferenceException(string.Format("partTypeConfigId: {0} doesn't have partType", partTypeConfigId));
            return manager.GetExtensionConfigurations<ChargingPolicyPartConfig>(partType.PartTypeExtensionName);
        }

        #endregion

        #region Validation Methods

        private void ValidateServiceTypeToEdit(ServiceTypeToEdit serviceType)
        {
            ServiceType serviceTypeEntity = this.GetServiceType(serviceType.ServiceTypeId);

            if (serviceTypeEntity == null)
                throw new DataIntegrityValidationException(String.Format("ServiceType '{0}' does not exist", serviceType.ServiceTypeId));
            if(serviceType.ChargingPolicyDefinitionSettings == null)
                  throw new DataIntegrityValidationException(String.Format("ChargingPolicyDefinitionSettings is null"));

             if(serviceType.ChargingPolicyDefinitionSettings.PartDefinitions == null)
                  throw new DataIntegrityValidationException(String.Format("No parts definitions"));

            foreach(var part in serviceType.ChargingPolicyDefinitionSettings.PartDefinitions)
            {
                if(serviceType.ChargingPolicyDefinitionSettings.PartDefinitions.Count(x=>x.PartTypeId == part.PartTypeId)>1)
                    throw new DataIntegrityValidationException(String.Format("Same PartTypeId {0} used more than once", part.PartTypeId));
            }
        }
        #endregion

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IServiceTypeDataManager _dataManager = BEDataManagerFactory.GetDataManager<IServiceTypeDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreServiceTypesUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region Private Methods

        Dictionary<int, ServiceType> GetCachedServiceTypes()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetServiceTypes", () =>
            {
                IServiceTypeDataManager dataManager = BEDataManagerFactory.GetDataManager<IServiceTypeDataManager>();
                IEnumerable<ServiceType> serviceTypes = dataManager.GetServiceTypes();
                return serviceTypes.ToDictionary(kvp => kvp.ServiceTypeId, kvp => kvp);
            });
        }
        #endregion

        #region Mappers

        private ServiceTypeDetail ServiceTypeDetailMapper(ServiceType serviceType)
        {
            return new ServiceTypeDetail()
            {
                Entity = serviceType,
            };
        }

        private ServiceTypeInfo ServiceTypeInfoMapper(ServiceType serviceType)
        {
            return new ServiceTypeInfo() { ServiceTypeId = serviceType.ServiceTypeId, Name = serviceType.Name };
        }

        #endregion
    }
}

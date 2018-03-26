using Retail.BusinessEntity.Data;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

namespace Retail.BusinessEntity.Business
{
    public class ServiceTypeManager : BaseBusinessEntityManager
    {
        #region Public Methods

        public Vanrise.Entities.IDataRetrievalResult<ServiceTypeDetail> GetFilteredServiceTypes(Vanrise.Entities.DataRetrievalInput<ServiceTypeQuery> input)
        {
            Dictionary<Guid, ServiceType> cachedServiceTypes = this.GetCachedServiceTypesWithHidden();

            Func<ServiceType, bool> filterExpression = (serviceType) =>
                (input.Query.Name == null || serviceType.Name.ToLower().Contains(input.Query.Name.ToLower()));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, cachedServiceTypes.ToBigResult(input, filterExpression, ServiceTypeDetailMapper));
        }

        public ServiceType GetServiceType(Guid serviceTypeId)
        {
            Dictionary<Guid, ServiceType> cachedServiceTypes = this.GetCachedServiceTypesWithHidden();
            return cachedServiceTypes.GetRecord(serviceTypeId);
        }

        public List<ServiceType> GetServiceTypes(Guid accountBEDefinitionId)
        {
            Dictionary<Guid, List<ServiceType>> cachedServiceTypes = this.GetCachedServiceTypesByAccountBEDefinitionId();
            return cachedServiceTypes.GetRecord(accountBEDefinitionId);
        }

        public string GetServiceTypeName(Guid serviceTypeId)
        {
            ServiceType serviceType = this.GetServiceType(serviceTypeId);
            return (serviceType != null) ? serviceType.Title : null;
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
                IdentificationRuleDefinitionId = updatedServiceType.IdentificationRuleDefinitionId,
                InitialStatusId = updatedServiceType.InitialStatusId,
                ExtendedSettings = updatedServiceType.ExtendedSettings
            };
            if (serviceType.Settings.ChargingPolicyDefinitionSettings != null)
                serviceTypeSettings.ChargingPolicyDefinitionSettings.ChargingPolicyEditor = serviceType.Settings.ChargingPolicyDefinitionSettings.ChargingPolicyEditor;


            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<ServiceTypeDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IServiceTypeDataManager dataManager = BEDataManagerFactory.GetDataManager<IServiceTypeDataManager>();

            if (dataManager.Update(updatedServiceType.ServiceTypeId, updatedServiceType.Title, updatedServiceType.AccountBEDefinitionId, serviceTypeSettings))
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

        public IEnumerable<ServiceTypeInfo> GetServiceTypesInfo(ServiceTypeInfoFilter filter)
        {
            Dictionary<Guid, ServiceType> cachedServiceTypes = null;

            Func<ServiceType, bool> filterExpression = null;
            if (filter != null)
            {
                if (filter.IncludeHiddenServiceTypes)
                    cachedServiceTypes = this.GetCachedServiceTypesWithHidden();

                filterExpression = (serviceType) =>
                {
                    if (filter.AccountBEDefinitionId.HasValue && filter.AccountBEDefinitionId.Value != serviceType.AccountBEDefinitionId)
                        return false;

                    if (filter.ExcludedServiceTypeIds != null && filter.ExcludedServiceTypeIds.Count() > 0 && filter.ExcludedServiceTypeIds.Contains(serviceType.ServiceTypeId))
                        return false;

                    if (filter.Filters != null && !CheckIfFilterIsMatch(serviceType, filter.Filters))
                        return false;

                    return true;
                };
            }

            if (cachedServiceTypes == null)
                cachedServiceTypes = this.GetCachedServiceTypes();

            return cachedServiceTypes.MapRecords(ServiceTypeInfoMapper, filterExpression).OrderBy(x => x.Title);
        }

        public ChargingPolicyDefinitionSettings GetServiceTypeChargingPolicyDefinitionSettings(Guid serviceTypeId)
        {
            ServiceType serviceType = this.GetServiceType(serviceTypeId);
            return (serviceType != null && serviceType.Settings != null) ? serviceType.Settings.ChargingPolicyDefinitionSettings : null;
        }

        public IEnumerable<ServiceTypeExtendedSettingsConfig> GetServiceTypeExtendedSettingsTemplateConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<ServiceTypeExtendedSettingsConfig>(ServiceTypeExtendedSettingsConfig.EXTENSION_TYPE);
        }

        private struct GetAccountServiceGenericFieldsCacheName
        {
            public Guid ServiceTypeId { get; set; }
        }

        public AccountServiceGenericField GetAccountServiceGenericField(Guid serviceTypeId, string fieldName)
        {
            return GetAccountServiceGenericFields(serviceTypeId).GetRecord(fieldName);
        }

        public Dictionary<string, AccountServiceGenericField> GetAccountServiceGenericFields(Guid serviceTypeId)
        {
            var cacheName = new GetAccountServiceGenericFieldsCacheName { ServiceTypeId = serviceTypeId };
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(cacheName,
                () =>
                {
                    List<AccountServiceGenericField> fields = new List<AccountServiceGenericField>();
                    FillAccountServiceCommonGenericFields(fields);
                    var serviceType = GetServiceType(serviceTypeId);
                    if (serviceType == null)
                        throw new NullReferenceException(String.Format("serviceType '{0}'", serviceTypeId));
                    if (serviceType.Settings != null && serviceType.Settings.ExtendedSettings != null)
                    {
                        var fieldDefinitions = serviceType.Settings.ExtendedSettings.GetFieldDefinitions();
                        if (fieldDefinitions != null)
                        {
                            fields.AddRange(fieldDefinitions.Select(fldDefinition => new AccountServiceTypeGenericField(serviceType, fldDefinition)));
                        }
                    }
                    return fields.ToDictionary(fld => fld.Name, fld => fld);
                });
        }

        public ChargingPolicyDefinitionSettings GetChargingPolicyDefinitionSettings(Guid serviceTypeId)
        {
            ServiceType serviceType = this.GetServiceType(serviceTypeId);

            if (serviceType == null)
                throw new NullReferenceException(string.Format("serviceType of serviceTypeId: {0}", serviceTypeId));

            if (serviceType.Settings == null)
                throw new NullReferenceException(string.Format("serviceType.Settings of serviceTypeId {0}", serviceTypeId));

            return serviceType.Settings.ChargingPolicyDefinitionSettings;
        }

        #endregion

        #region Validation Methods

        private void ValidateServiceTypeToEdit(ServiceTypeToEdit serviceType)
        {
            ServiceType serviceTypeEntity = this.GetServiceType(serviceType.ServiceTypeId);

            if (serviceTypeEntity == null)
                throw new DataIntegrityValidationException(String.Format("ServiceType '{0}' does not exist", serviceType.ServiceTypeId));
            if (serviceType.ChargingPolicyDefinitionSettings == null)
                throw new DataIntegrityValidationException(String.Format("ChargingPolicyDefinitionSettings is null"));

            if (serviceType.ChargingPolicyDefinitionSettings.PartDefinitions == null)
                throw new DataIntegrityValidationException(String.Format("No parts definitions"));

            foreach (var part in serviceType.ChargingPolicyDefinitionSettings.PartDefinitions)
            {
                if (serviceType.ChargingPolicyDefinitionSettings.PartDefinitions.Count(x => x.PartTypeId == part.PartTypeId) > 1)
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

        internal Dictionary<Guid, ServiceType> GetCachedServiceTypesWithHidden()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetServiceTypesWithHidden", () =>
            {
                IServiceTypeDataManager dataManager = BEDataManagerFactory.GetDataManager<IServiceTypeDataManager>();
                IEnumerable<ServiceType> serviceTypes = dataManager.GetServiceTypes();
                return serviceTypes.ToDictionary(kvp => kvp.ServiceTypeId, kvp => kvp);
            });
        }

        Dictionary<Guid, ServiceType> GetCachedServiceTypes()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedAccountTypes", () =>
            {
                List<ServiceType> includedServiceTypes = new List<ServiceType>();
                VRRetailBEVisibilityManager retailBEVisibilityManager = new VRRetailBEVisibilityManager();
                Dictionary<Guid, VRRetailBEVisibilityAccountDefinitionServiceType> visibleServiceTypesById;

                IEnumerable<ServiceType> allServiceTypes = this.GetCachedServiceTypesWithHidden().Values;

                if (retailBEVisibilityManager.ShouldApplyServiceTypesVisibility(out visibleServiceTypesById))
                {
                    foreach (var itm in allServiceTypes)
                    {
                        if (visibleServiceTypesById.ContainsKey(itm.ServiceTypeId))
                            includedServiceTypes.Add(itm);
                    }
                }
                else
                {
                    includedServiceTypes = allServiceTypes.ToList();
                }

                return includedServiceTypes.ToDictionary(kvp => kvp.ServiceTypeId, kvp => kvp);
            });
        }

        Dictionary<Guid, List<ServiceType>> GetCachedServiceTypesByAccountBEDefinitionId()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedServiceTypesByAccountDefinitionId", () =>
            {
                Dictionary<Guid, List<ServiceType>> result = new Dictionary<Guid, List<ServiceType>>();
                IEnumerable<ServiceType> allServiceTypes = this.GetCachedServiceTypes().Values;
                foreach (ServiceType serviceType in allServiceTypes)
                {
                    List<ServiceType> serviceTypes = result.GetOrCreateItem(serviceType.AccountBEDefinitionId);
                    serviceTypes.Add(serviceType);
                }
                return result;
            });
        }

        private bool CheckIfFilterIsMatch(ServiceType entityDefinition, List<IServiceTypeFilter> filters)
        {
            PackageDefinitionServiceTypeFilterContext context = new PackageDefinitionServiceTypeFilterContext { entityDefinition = entityDefinition };
            foreach (var filter in filters)
            {
                if (!filter.IsMatched(context))
                    return false;
            }
            return true;
        }

        private void FillAccountServiceCommonGenericFields(List<AccountServiceGenericField> fields)
        {
            //fields.Add(new AccountServiceStatusGenericField());
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
            return new ServiceTypeInfo() { ServiceTypeId = serviceType.ServiceTypeId, Title = serviceType.Title };
        }

        #endregion

        #region IBusinessEntityManager

        public override List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context)
        {
            var cachedPackages = GetCachedServiceTypes();
            if (cachedPackages != null)
                return cachedPackages.Values.Select(itm => itm as dynamic).ToList();
            else
                return null;
        }

        public override dynamic GetEntity(IBusinessEntityGetByIdContext context)
        {
            return GetServiceType(context.EntityId);
        }

        public override string GetEntityDescription(IBusinessEntityDescriptionContext context)
        {
            return GetServiceTypeName(new Guid(context.EntityId.ToString()));
        }

        public override dynamic GetEntityId(IBusinessEntityIdContext context)
        {
            var serviceType = context.Entity as ServiceType;
            return serviceType.ServiceTypeId;
        }

        public override bool IsCacheExpired(IBusinessEntityIsCacheExpiredContext context, ref DateTime? lastCheckTime)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().IsCacheExpired(ref lastCheckTime);
        }

        public override IEnumerable<dynamic> GetIdsByParentEntityId(IBusinessEntityGetIdsByParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public override dynamic GetParentEntityId(IBusinessEntityGetParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public override dynamic MapEntityToInfo(IBusinessEntityMapToInfoContext context)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

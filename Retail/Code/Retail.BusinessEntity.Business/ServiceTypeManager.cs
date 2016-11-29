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
using Vanrise.GenericData.Entities;
namespace Retail.BusinessEntity.Business
{
    public class ServiceTypeManager : IBusinessEntityManager
    {
        #region Public Methods

        public Vanrise.Entities.IDataRetrievalResult<ServiceTypeDetail> GetFilteredServiceTypes(Vanrise.Entities.DataRetrievalInput<ServiceTypeQuery> input)
        {
            Dictionary<Guid, ServiceType> cachedServiceTypes = this.GetCachedServiceTypes();

            Func<ServiceType, bool> filterExpression = (serviceType) =>
                (input.Query.Name == null || serviceType.Name.ToLower().Contains(input.Query.Name.ToLower()));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, cachedServiceTypes.ToBigResult(input, filterExpression, ServiceTypeDetailMapper));
        }

        public IEnumerable<ServiceTypeInfo> GetServiceTypesInfo()
        {
            return this.GetCachedServiceTypes().MapRecords(ServiceTypeInfoMapper).OrderBy(x => x.Title);
        }

        public ServiceType GetServiceType(Guid serviceTypeId)
        {
            Dictionary<Guid, ServiceType> cachedServiceTypes = this.GetCachedServiceTypes();
            return cachedServiceTypes.GetRecord(serviceTypeId);
        }

        public string GetServiceTypeName(Guid serviceTypeId)
        {
            ServiceType serviceType = this.GetServiceType(serviceTypeId);
            return (serviceType != null) ? serviceType.Title : null;
        }

        public ChargingPolicyDefinitionSettings GetServiceTypeChargingPolicyDefinitionSettings(Guid serviceTypeId)
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
                IdentificationRuleDefinitionId = updatedServiceType.IdentificationRuleDefinitionId,
                InitialStatusId = updatedServiceType.InitialStatusId
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

        private struct GetAccountServiceGenericFieldsCacheName
        {
            public Guid ServiceTypeId { get; set; }
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
                    if(serviceType.Settings != null && serviceType.Settings.ExtendedSettings != null)
                    {
                        var fieldDefinitions = serviceType.Settings.ExtendedSettings.GetFieldDefinitions();
                        if(fieldDefinitions != null)
                        {
                            fields.AddRange(fieldDefinitions.Select(fldDefinition => new AccountServiceTypeGenericField(serviceType, fldDefinition)));
                        }
                    }
                    return fields.ToDictionary(fld => fld.Name, fld => fld);
                });
        }

        public AccountServiceGenericField GetAccountServiceGenericField(Guid serviceTypeId, string fieldName)
        {
            return GetAccountServiceGenericFields(serviceTypeId).GetRecord(fieldName);
        }

        void FillAccountServiceCommonGenericFields(List<AccountServiceGenericField> fields)
        {
            fields.Add(new AccountServiceStatusGenericField());
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

        Dictionary<Guid, ServiceType> GetCachedServiceTypes()
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
            return new ServiceTypeInfo() { ServiceTypeId = serviceType.ServiceTypeId, Title = serviceType.Title };
        }

        #endregion

        #region IBusinessEntityManager

        public List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context)
        {
            var cachedPackages = GetCachedServiceTypes();
            if (cachedPackages != null)
                return cachedPackages.Values.Select(itm => itm as dynamic).ToList();
            else
                return null;
        }

        public dynamic GetEntity(IBusinessEntityGetByIdContext context)
        {
            return GetServiceType(context.EntityId);
        }

        public string GetEntityDescription(IBusinessEntityDescriptionContext context)
        {
            return GetServiceTypeName(new Guid(context.EntityId.ToString()));
        }

        public bool IsCacheExpired(IBusinessEntityIsCacheExpiredContext context, ref DateTime? lastCheckTime)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().IsCacheExpired(ref lastCheckTime);
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

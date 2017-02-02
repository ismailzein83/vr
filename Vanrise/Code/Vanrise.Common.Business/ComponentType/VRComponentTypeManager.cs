using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common;
using Vanrise.Common.Data;
using Vanrise.Caching;

namespace Vanrise.Common.Business
{
    public class VRComponentTypeManager
    {
        static CacheManager s_cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>();

        public IDataRetrievalResult<VRComponentTypeDetail> GetFilteredVRComponentTypes(DataRetrievalInput<VRComponentTypeQuery> input)
        {
            var allVRComponentTypes = GetCachedComponentTypes();
            Func<VRComponentType, bool> filterExpression = (x) =>
            {
                if (input.Query.Name != null && !x.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;
                if (x.Settings.VRComponentTypeConfigId != input.Query.ExtensionConfigId)
                    return false;
                return true;
            };
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allVRComponentTypes.ToBigResult(input, filterExpression, VRComponentTypeDetailMapper));
        }

        public VRComponentType GetComponentType(Guid vrComponentTypeId)
        {
            Dictionary<Guid, VRComponentType> cachedVRComponentTypes = this.GetCachedComponentTypes();
            return cachedVRComponentTypes.GetRecord(vrComponentTypeId);
        }

        public InsertOperationOutput<VRComponentTypeDetail> AddVRComponentType(VRComponentType componentType)
        {
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<VRComponentTypeDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            IVRComponentTypeDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRComponentTypeDataManager>();

            componentType.VRComponentTypeId = Guid.NewGuid();

            if (dataManager.Insert(componentType))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = VRComponentTypeDetailMapper(componentType);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }
        public UpdateOperationOutput<VRComponentTypeDetail> UpdateVRComponentType(VRComponentType componentType)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<VRComponentTypeDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IVRComponentTypeDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRComponentTypeDataManager>();

            if (dataManager.Update(componentType))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = VRComponentTypeDetailMapper(this.GetComponentType(componentType.VRComponentTypeId));
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public IEnumerable<VRComponentTypeConfig> GetVRComponentTypeExtensionConfigs()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<VRComponentTypeConfig>(VRComponentTypeConfig.EXTENSION_TYPE);
        }
        public VRComponentTypeConfig GetVRComponentTypeExtensionConfigById(Guid configId)
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfiguration<VRComponentTypeConfig>(configId, VRComponentTypeConfig.EXTENSION_TYPE);
        }

        public Q GetComponentType<T, Q>(Guid componentTypeId)
            where T : VRComponentTypeSettings
            where Q : VRComponentType<T>
        {
            var cacheName = new GetComponentTypeCacheName
            {
                ComponentTypeId = componentTypeId,
                CLRType = typeof(T)
            };
            return s_cacheManager.GetOrCreateObject(cacheName, () =>
            {
                var componentTypeEntity = GetCachedComponentTypes().GetRecord(componentTypeId);
                if (componentTypeEntity == null)
                    throw new NullReferenceException(String.Format("componentTypeEntity '{0}'", componentTypeId));
                if (componentTypeEntity.Settings == null)
                    throw new NullReferenceException(String.Format("componentTypeEntity.Settings '{0}'", componentTypeId));
                T settingsAsT = componentTypeEntity.Settings as T;
                if (settingsAsT == null)
                    throw new NullReferenceException(String.Format("componentTypeEntity.Settings is not of type '{0}'. it is of type '{1}'", typeof(T), componentTypeEntity.Settings.GetType()));
                var componentType = Activator.CreateInstance<Q>();
                componentType.VRComponentTypeId = componentTypeEntity.VRComponentTypeId;
                componentType.Name = componentTypeEntity.Name;
                componentType.Settings = settingsAsT;
                return componentType;
            });
        }

        public List<VRComponentType<T>> GetComponentTypes<T>()
            where T : VRComponentTypeSettings
        {
            return GetCachedComponentTypes().FindAllRecords(itm => itm.Settings is T).Select(itm => new VRComponentType<T>
            {
                VRComponentTypeId = itm.VRComponentTypeId,
                Name = itm.Name,
                Settings = itm.Settings as T
            }).ToList();
        }

        public List<Q> GetComponentTypes<T, Q>()
            where T : VRComponentTypeSettings
            where Q : VRComponentType<T>
        {
            return GetCachedComponentTypes().FindAllRecords(itm => itm.Settings is T).Select(
                itm =>
                {
                    var componentTypeAsQ = Activator.CreateInstance<Q>();
                    componentTypeAsQ.VRComponentTypeId = itm.VRComponentTypeId;
                    componentTypeAsQ.Name = itm.Name;
                    componentTypeAsQ.Settings = itm.Settings as T;
                    return componentTypeAsQ;
                }).ToList();
        }

        public T GetComponentTypeSettings<T>(Guid componentTypeId) where T : VRComponentTypeSettings
        {
            var componentType = GetComponentType<T, VRComponentType<T>>(componentTypeId);
            if (componentType == null)
                throw new NullReferenceException(String.Format("componentType '{0}'", componentTypeId));
            return componentType.Settings;
        }

        public T GetCachedOrCreate<T>(Object cacheName, Func<T> createObject)
        {
            return s_cacheManager.GetOrCreateObject(cacheName, createObject);
        }

        private Dictionary<Guid, VRComponentType> GetCachedComponentTypes()
        {
            return s_cacheManager.GetOrCreateObject("GetCachedComponentTypes", () =>
            {
                IVRComponentTypeDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRComponentTypeDataManager>();
                IEnumerable<VRComponentType> componentTypes = dataManager.GetComponentTypes();

                return componentTypes.ToDictionary(ct => ct.VRComponentTypeId, ct => ct);
            });
        }

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IVRComponentTypeDataManager _dataManager = CommonDataManagerFactory.GetDataManager<IVRComponentTypeDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired()
            {
                return _dataManager.AreVRComponentTypeUpdated(ref _updateHandle);
            }
        }

        private struct GetComponentTypeCacheName
        {
            public Guid ComponentTypeId { get; set; }

            public Type CLRType { get; set; }
        }

        #endregion

        #region Mappers

        private VRComponentTypeDetail VRComponentTypeDetailMapper(VRComponentType componentType)
        {
            VRComponentTypeDetail componentTypeDetail = new VRComponentTypeDetail()
            {
                Entity = componentType
            };
            return componentTypeDetail;
        }

        #endregion
    }
}

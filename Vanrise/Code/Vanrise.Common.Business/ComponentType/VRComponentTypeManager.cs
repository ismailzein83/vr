using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Entities;
using Vanrise.Common.Data;
using Vanrise.GenericData.Entities;

namespace Vanrise.Common.Business
{
    public class VRComponentTypeManager : BaseBusinessEntityManager
    {

        #region Ctor/Variables

        static CacheManager s_cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>();

        #endregion

        #region Public Methods

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

        public string GetVRComponentTypeName(VRComponentType vrComponentType)
        {
            return (vrComponentType != null) ? vrComponentType.Name : null;
        }

        public IEnumerable<ComponentTypeInfo> GetComponentTypeInfo(ComponentTypeInfoFilter filter)
        {
            var cachedVRComponentTypes = this.GetCachedComponentTypes();
            Func<VRComponentType, bool> filterExpression = null;

            if (filter != null)
            {
                filterExpression = (item) =>
                {
                    if (filter.ExcludedIds != null && filter.ExcludedIds.Contains(item.VRComponentTypeId))
                        return false;

                    if (filter.Filters != null && !CheckIfFilterIsMatch(item, filter.Filters))
                        return false;

                    return true;
                };
            }

            return cachedVRComponentTypes.MapRecords(VRComponentTypeInfoMapper, filterExpression).OrderBy(x => x.Name);
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
                VRActionLogger.Current.TrackAndLogObjectAdded(new VRComponentTypeLoggableEntity(componentType.Settings.VRComponentTypeConfigId), componentType);
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
                VRActionLogger.Current.TrackAndLogObjectUpdated(new VRComponentTypeLoggableEntity(componentType.Settings.VRComponentTypeConfigId), componentType);
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = VRComponentTypeDetailMapper(this.GetComponentType(componentType.VRComponentTypeId));
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }
        public string GetVRComponentTypeExtensionConfigName(Guid configId)
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurationTitle<VRComponentTypeConfig>(configId, VRComponentTypeConfig.EXTENSION_TYPE);
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

        private bool CheckIfFilterIsMatch(VRComponentType componentType, List<IComponentTypeFilter> filters)
        {
            ComponentTypeFilterContext context = new ComponentTypeFilterContext { componentType = componentType };
            foreach (var filter in filters)
            {
                if (!filter.IsMatched(context))
                    return false;
            }
            return true;
        }
        private struct GetComponentTypeCacheName
        {
            public Guid ComponentTypeId { get; set; }

            public string VRComponentTypeFQTN { get; set; }

            public string VRComponentTypeSettingsFQTN { get; set; }
        }

        public Q GetComponentType<T, Q>(Guid componentTypeId)
            where T : VRComponentTypeSettings
            where Q : VRComponentType<T>
        {
            var cacheName = new GetComponentTypeCacheName
            {
                ComponentTypeId = componentTypeId,
                VRComponentTypeFQTN = typeof(Q).AssemblyQualifiedName,
                VRComponentTypeSettingsFQTN = typeof(T).AssemblyQualifiedName
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

        private struct GetCachedComponentTypesCacheName
        {
            public string VRComponentTypeFQTN { get; set; }

            public string VRComponentTypeSettingsFQTN { get; set; }
        }

        public Dictionary<Guid, Q> GetCachedComponentTypes<T, Q>()
            where T : VRComponentTypeSettings
            where Q : VRComponentType<T>
        {
            var cacheName = new GetCachedComponentTypesCacheName
            {
                VRComponentTypeFQTN = typeof(Q).AssemblyQualifiedName,
                VRComponentTypeSettingsFQTN = typeof(T).AssemblyQualifiedName
            };

            return s_cacheManager.GetOrCreateObject(cacheName, () =>
            {
                return GetCachedComponentTypes().FindAllRecords(itm => itm.Settings is T).Select(
                   itm =>
                   {
                       var componentTypeAsQ = Activator.CreateInstance<Q>();
                       componentTypeAsQ.VRComponentTypeId = itm.VRComponentTypeId;
                       componentTypeAsQ.Name = itm.Name;
                       componentTypeAsQ.Settings = itm.Settings as T;
                       return componentTypeAsQ;
                   }).ToDictionary(itm => itm.VRComponentTypeId, itm => itm);
            });
        }

        public T GetComponentTypeSettings<T>(Guid componentTypeId) where T : VRComponentTypeSettings
        {
            var componentType = GetComponentType<T, VRComponentType<T>>(componentTypeId);
            if (componentType == null)
                throw new NullReferenceException(String.Format("componentType '{0}'", componentTypeId));
            return componentType.Settings;
        }

        #endregion

        #region Private Methods

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

        #endregion

        #region Private Classes

        public class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IVRComponentTypeDataManager _dataManager = CommonDataManagerFactory.GetDataManager<IVRComponentTypeDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired()
            {
                return _dataManager.AreVRComponentTypeUpdated(ref _updateHandle);
            }
        }

        private class VRComponentTypeLoggableEntity : VRLoggableEntityBase
        {

            Guid _vrComponentTypeConfigId;

            public VRComponentTypeLoggableEntity(Guid vrComponentTypeConfigId)
            {
                _vrComponentTypeConfigId = vrComponentTypeConfigId;
            }

            static VRComponentTypeManager s_vrComponentTypeManager = new VRComponentTypeManager();
            public override string EntityUniqueName
            {
                get { return String.Format("VR_Common_ComponentType_{0}", _vrComponentTypeConfigId); }
            }

            public override string ModuleName
            {
                get { return "Common"; }
            }

            public override string EntityDisplayName
            {
                get { return s_vrComponentTypeManager.GetVRComponentTypeExtensionConfigName(_vrComponentTypeConfigId); }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "VR_Common_ComponentType_ViewHistoryItem"; }
            }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {

                VRComponentType vrComponentType = context.Object.CastWithValidate<VRComponentType>("context.Object");
                return vrComponentType.VRComponentTypeId;

            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {

                VRComponentType vrComponentType = context.Object.CastWithValidate<VRComponentType>("context.Object");
                return s_vrComponentTypeManager.GetVRComponentTypeName(vrComponentType);

            }
        }

        #endregion

        #region Mappers
        ComponentTypeInfo VRComponentTypeInfoMapper(VRComponentType componentType)
        {
            return new ComponentTypeInfo()
            {
                VRComponentTypeId = componentType.VRComponentTypeId,
                Name = componentType.Name,
             
            };
        }
        private VRComponentTypeDetail VRComponentTypeDetailMapper(VRComponentType componentType)
        {
            VRComponentTypeDetail componentTypeDetail = new VRComponentTypeDetail()
            {
                Entity = componentType
            };
            return componentTypeDetail;
        }

        #endregion

        #region IBusinessEntityManager

        public override List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context)
        {
            throw new NotImplementedException();
        }

        public override dynamic GetEntity(IBusinessEntityGetByIdContext context)
        {
            throw new NotImplementedException();
        }

        public override dynamic GetEntityId(IBusinessEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public override string GetEntityDescription(IBusinessEntityDescriptionContext context)
        {
            throw new NotImplementedException();
        }

        public override dynamic MapEntityToInfo(IBusinessEntityMapToInfoContext context)
        {
            throw new NotImplementedException();
        }

        public override bool IsCacheExpired(IBusinessEntityIsCacheExpiredContext context, ref DateTime? lastCheckTime)
        {
            throw new NotImplementedException();
        }

        public override dynamic GetParentEntityId(IBusinessEntityGetParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<dynamic> GetIdsByParentEntityId(IBusinessEntityGetIdsByParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

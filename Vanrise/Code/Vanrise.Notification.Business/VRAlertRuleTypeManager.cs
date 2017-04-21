using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Notification.Data;
using Vanrise.Notification.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Security.Entities;

namespace Vanrise.Notification.Business
{
    public class VRAlertRuleTypeManager
    {
        #region Public Methods

        public VRAlertRuleType GetVRAlertRuleType(Guid vrAlertRuleTypeId)
        {
            Dictionary<Guid, VRAlertRuleType> cachedVRAlertRuleTypes = this.GetCachedVRAlertRuleTypes();
            return cachedVRAlertRuleTypes.GetRecord(vrAlertRuleTypeId);
        }

        public string GetVRAlertRuleTypeName(Guid vrAlertRuleTypeId)
        {
            var vralertRuleType = GetVRAlertRuleType(vrAlertRuleTypeId);
            if (vralertRuleType == null)
                return null;
            return vralertRuleType.Name;
        }

        public T GetVRAlertRuleTypeSettings<T>(Guid vrAlertRuleTypeId) where T : VRAlertRuleTypeSettings
        {
            var alertRuleType = GetVRAlertRuleType(vrAlertRuleTypeId);
            if (alertRuleType == null)
                throw new NullReferenceException(String.Format("alertRuleType '{0}'", vrAlertRuleTypeId));
            if (alertRuleType.Settings == null)
                throw new NullReferenceException(String.Format("alertRuleType.Settings '{0}'", vrAlertRuleTypeId));
            T settingsAsT = alertRuleType.Settings as T;
            if (settingsAsT == null)
                throw new Exception(String.Format("alertRuleType.Settings is not of type '{0}'. it is of type '{1}'", typeof(T), alertRuleType.Settings.GetType()));
            return settingsAsT;
        }

        public IDataRetrievalResult<VRAlertRuleTypeDetail> GetFilteredVRAlertRuleTypes(DataRetrievalInput<VRAlertRuleTypeQuery> input)
        {
            var allVRAlertRuleTypes = GetCachedVRAlertRuleTypes();
            Func<VRAlertRuleType, bool> filterExpression = (x) => (input.Query.Name == null || x.Name.ToLower().Contains(input.Query.Name.ToLower()));
            VRActionLogger.Current.LogGetFilteredAction(VRAlertRuleTypeLoggableEntity.Instance, input);
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allVRAlertRuleTypes.ToBigResult(input, filterExpression, VRAlertRuleTypeDetailMapper));
        }

        public Vanrise.Entities.InsertOperationOutput<VRAlertRuleTypeDetail> AddVRAlertRuleType(VRAlertRuleType vrAlertRuleTypeItem)
        {
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<VRAlertRuleTypeDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            IVRAlertRuleTypeDataManager dataManager = NotificationDataManagerFactory.GetDataManager<IVRAlertRuleTypeDataManager>();

            vrAlertRuleTypeItem.VRAlertRuleTypeId = Guid.NewGuid();

            if (dataManager.Insert(vrAlertRuleTypeItem))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                VRActionLogger.Current.TrackAndLogObjectAdded(VRAlertRuleTypeLoggableEntity.Instance, vrAlertRuleTypeItem);
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = VRAlertRuleTypeDetailMapper(vrAlertRuleTypeItem);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<VRAlertRuleTypeDetail> UpdateVRAlertRuleType(VRAlertRuleType vrAlertRuleTypeItem)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<VRAlertRuleTypeDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IVRAlertRuleTypeDataManager dataManager = NotificationDataManagerFactory.GetDataManager<IVRAlertRuleTypeDataManager>();

            if (dataManager.Update(vrAlertRuleTypeItem))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                VRActionLogger.Current.TrackAndLogObjectUpdated(VRAlertRuleTypeLoggableEntity.Instance, vrAlertRuleTypeItem);
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = VRAlertRuleTypeDetailMapper(this.GetVRAlertRuleType(vrAlertRuleTypeItem.VRAlertRuleTypeId));
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public IEnumerable<VRAlertRuleTypeConfig> GetVRAlertRuleTypeSettingsExtensionConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<VRAlertRuleTypeConfig>(VRAlertRuleTypeConfig.EXTENSION_TYPE);
        }

        public IEnumerable<VRAlertRuleTypeInfo> GetVRAlertRuleTypesInfo(VRAlertRuleTypeFilter filter)
        {
            Func<VRAlertRuleType, bool> filterExpression = (alertRuleType) =>
            {
                if (filter != null)
                {
                    if (filter.Filters != null)
                    {
                        foreach (IVRAlertRuleTypeFilter dataAnalysisDefinitionFilter in filter.Filters)
                        {
                            if (!dataAnalysisDefinitionFilter.IsMatch(alertRuleType))
                                return false;
                        }
                    }
                }
                return true;
            };

            return this.GetCachedVRAlertRuleTypes().MapRecords(VRAlertRuleTypeInfoMapper, filterExpression).OrderBy(x => x.Name);
        }

        #endregion

        #region Caching

        static CacheManager s_cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>();

        public T GetCachedOrCreate<T>(Object cacheName, Func<T> createObject)
        {
            return s_cacheManager.GetOrCreateObject(cacheName, createObject);
        }
        internal class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IVRAlertRuleTypeDataManager _dataManager = NotificationDataManagerFactory.GetDataManager<IVRAlertRuleTypeDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreVRAlertRuleTypeUpdated(ref _updateHandle);
            }
        }
        #endregion

        #region Private Classes

        

        private class VRAlertRuleTypeLoggableEntity : VRLoggableEntityBase
        {
            public static VRAlertRuleTypeLoggableEntity Instance = new VRAlertRuleTypeLoggableEntity();

            private VRAlertRuleTypeLoggableEntity()
            {

            }

            static VRAlertRuleTypeManager s_vrAlertRuleTypeManager = new VRAlertRuleTypeManager();

            public override string EntityUniqueName
            {
                get { return "VR_Notification_AlertRuleType"; }
            }

            public override string ModuleName
            {
                get { return "Notification"; }
            }

            public override string EntityDisplayName
            {
                get { return "Alert Rule Type"; }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "VR_Notification_AlertRuleType_ViewHistoryItem"; }
            }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                VRAlertRuleType vrAlertRuleType = context.Object.CastWithValidate<VRAlertRuleType>("context.Object");
                return vrAlertRuleType.VRAlertRuleTypeId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                VRAlertRuleType vrAlertRuleType = context.Object.CastWithValidate<VRAlertRuleType>("context.Object");
                return s_vrAlertRuleTypeManager.GetVRAlertRuleTypeName(vrAlertRuleType.VRAlertRuleTypeId);
            }
        }

        #endregion

        #region Private Methods

        public Dictionary<Guid, VRAlertRuleType> GetCachedVRAlertRuleTypes()
        {
            return s_cacheManager.GetOrCreateObject("GetVRAlertRuleTypes",
               () =>
               {
                   IVRAlertRuleTypeDataManager dataManager = NotificationDataManagerFactory.GetDataManager<IVRAlertRuleTypeDataManager>();
                   return dataManager.GetVRAlertRuleTypes().ToDictionary(x => x.VRAlertRuleTypeId, x => x);
               });
        }

        private bool DoesUserHaveAccess(int userId, VRAlertRuleType alertRuleType, Func<AlertRuleTypeSecurity, RequiredPermissionSettings> getRequiredPermissionSetting)
        {
            if (alertRuleType != null && alertRuleType.Settings != null && alertRuleType.Settings.Security != null && getRequiredPermissionSetting(alertRuleType.Settings.Security) != null)
                return ContextFactory.GetContext().IsAllowed(getRequiredPermissionSetting(alertRuleType.Settings.Security), userId);
            else
                return true;
        }

        #endregion

        #region Mappers

        public VRAlertRuleTypeDetail VRAlertRuleTypeDetailMapper(VRAlertRuleType vrAlertRuleType)
        {
            VRAlertRuleTypeDetail vrAlertRuleTypeDetail = new VRAlertRuleTypeDetail()
            {
                Entity = vrAlertRuleType
            };
            return vrAlertRuleTypeDetail;
        }

        public VRAlertRuleTypeInfo VRAlertRuleTypeInfoMapper(VRAlertRuleType vrAlertRuleType)
        {
            VRAlertRuleTypeInfo vrAlertRuleTypeInfo = new VRAlertRuleTypeInfo()
            {
                VRAlertRuleTypeId = vrAlertRuleType.VRAlertRuleTypeId,
                Name = vrAlertRuleType.Name
            };
            return vrAlertRuleTypeInfo;
        }

        #endregion

        #region Security

        public HashSet<Guid> GetAllowedRuleTypeIds()
        {
            var alertRuleTypes = GetCachedVRAlertRuleTypes();
            int userId = ContextFactory.GetContext().GetLoggedInUserId();
            HashSet<Guid> allowedRuleTypeIds = new HashSet<Guid>();

            foreach (var a in alertRuleTypes)
            {
                if (DoesUserHaveViewAccess(userId, a.Value))
                    allowedRuleTypeIds.Add(a.Key);
            }
            return allowedRuleTypeIds;
        }

        public bool DoesUserHaveAddAccess()
        {
            var alertRuleTypes = GetCachedVRAlertRuleTypes().Values;
            int userId = ContextFactory.GetContext().GetLoggedInUserId();

            foreach (var a in alertRuleTypes)
            {
                if (DoesUserHaveAddAccess(userId, a))
                    return true;
            }
            return false;
        }
        public bool DoesUserHaveAddAccess(Guid alertRuleTypeId)
        {
            int userId = ContextFactory.GetContext().GetLoggedInUserId();
            return DoesUserHaveAddAccess(userId, alertRuleTypeId);
        }
        public bool DoesUserHaveViewAccess(int userId, VRAlertRuleType alertRuleType)
        {
            return DoesUserHaveAccess(userId, alertRuleType, (sec) => sec.ViewPermission);
        }
        public bool DoesUserHaveAddAccess(int userId, Guid alertRuleTypeId)
        {
            var alertRuleType = GetVRAlertRuleType(alertRuleTypeId);
            return DoesUserHaveAddAccess(userId, alertRuleType);
        }
        public bool DoesUserHaveAddAccess(int userId, VRAlertRuleType alertRuleType)
        {
            return DoesUserHaveAccess(userId, alertRuleType, (sec) => sec.AddPermission);
        }
        public bool DoesUserHaveEditAccess(Guid alertRuleTypeId)
        {
            int userId = ContextFactory.GetContext().GetLoggedInUserId();
            return DoesUserHaveEditAccess(userId,alertRuleTypeId);
        }
        public bool DoesUserHaveEditAccess(int userId, Guid alertRuleTypeId)
        {
            var alertRuleType = GetVRAlertRuleType(alertRuleTypeId);
            return DoesUserHaveAccess(userId, alertRuleType, (sec) => sec.EditPermission);
        }
        public bool DoesUserHaveViewAccess()
        {
            int userId = ContextFactory.GetContext().GetLoggedInUserId();
            return DoesUserHaveViewAccess(userId);
        }
        public bool DoesUserHaveViewAccess(int userId)
        {
            return GetAllowedRuleTypeIds().Count > 0;
        }

        #endregion

       
    }
}

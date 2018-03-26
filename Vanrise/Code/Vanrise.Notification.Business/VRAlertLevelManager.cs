using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.Notification.Data;
using Vanrise.Notification.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace Vanrise.Notification.Business
{
    public class VRAlertLevelManager : BaseBusinessEntityManager
    {
        VRNotificationTypeManager _vrnotificationTypeManager = new VRNotificationTypeManager();
        public VRAlertLevel GetAlertLevel(Guid alertLevelId)
        {
            Dictionary<Guid, VRAlertLevel> cachedAlertLevels = this.GetCachedAlertLevels();

            return cachedAlertLevels.GetRecord(alertLevelId);
        }

        public string GetAlertLevelName(Guid alertLevelId)
        {
            VRAlertLevel alertLevel = this.GetAlertLevel(alertLevelId);
            return (alertLevel != null) ? alertLevel.Name : null;
        }

        public int GetAlertLevelWeight(Guid alertLevelId)
        {
            VRAlertLevel alertLevel = this.GetAlertLevel(alertLevelId);
            alertLevel.ThrowIfNull<VRAlertLevel>("alertLevel", alertLevelId);
            alertLevel.Settings.ThrowIfNull<VRAlertLevelSettings>("alertLevel.Settings", alertLevelId);
            return alertLevel.Settings.Weight;
        }

        public IDataRetrievalResult<VRAlertLevelDetail> GetFilteredAlertLevels(DataRetrievalInput<VRAlertLevelQuery> input)
        {
            var allAlertLevels = GetCachedAlertLevels();
            Func<VRAlertLevel, bool> filterExpression = (x) =>
            {
                if (input.Query.BusinessEntityDefinitionIds != null && !input.Query.BusinessEntityDefinitionIds.Contains(x.BusinessEntityDefinitionId))
                    return false;
                if (input.Query.Name != null && !x.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;
                return true;
            };
            VRActionLogger.Current.LogGetFilteredAction(VRAlertLevelLoggableEntity.Instance, input);
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult<VRAlertLevelDetail>(input, allAlertLevels.ToBigResult(input, filterExpression, VRAlertLevelDetailMapper));
        }
        public Vanrise.Entities.InsertOperationOutput<VRAlertLevelDetail> AddAlertLevel(VRAlertLevel alertLevelItem)
        {
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<VRAlertLevelDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            IVRAlertLevelDataManager dataManager = NotificationDataManagerFactory.GetDataManager<IVRAlertLevelDataManager>();

            alertLevelItem.VRAlertLevelId = Guid.NewGuid();

            if (dataManager.Insert(alertLevelItem))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                VRActionLogger.Current.TrackAndLogObjectAdded(VRAlertLevelLoggableEntity.Instance, alertLevelItem);
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = VRAlertLevelDetailMapper(alertLevelItem);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<VRAlertLevelDetail> UpdateAlertLevel(VRAlertLevel alertLevelItem)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<VRAlertLevelDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IVRAlertLevelDataManager dataManager = NotificationDataManagerFactory.GetDataManager<IVRAlertLevelDataManager>();

            if (dataManager.Update(alertLevelItem))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                VRActionLogger.Current.TrackAndLogObjectUpdated(VRAlertLevelLoggableEntity.Instance, alertLevelItem);
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = VRAlertLevelDetailMapper(this.GetAlertLevel(alertLevelItem.VRAlertLevelId));
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }
        public IEnumerable<VRAlertLevel> GetAllAlertLevels()
        {
            return this.GetCachedAlertLevels().MapRecords(x => x).OrderBy(x => x.Name);
        }
        public IEnumerable<VRAlertLevelInfo> GetAlertLevelsInfo(VRAlertLevelInfoFilter filter)
        {
            Func<VRAlertLevel, bool> filterExpression = null;
            Guid businessEntityDefinitionId;
            if (filter != null)
            {
                if (!filter.BusinessEntityDefinitionId.HasValue && !filter.VRNotificationTypeId.HasValue)
                {
                    throw new Exception("BusinessEntityDefinitionId or VRNotificationTypeId should be specified.");
                }
                if (filter.BusinessEntityDefinitionId.HasValue)
                    businessEntityDefinitionId = filter.BusinessEntityDefinitionId.Value;
                else
                {
                    var notficationTypeSettings = _vrnotificationTypeManager.GetNotificationTypeSettings(filter.VRNotificationTypeId.Value);
                    businessEntityDefinitionId = notficationTypeSettings.VRAlertLevelDefinitionId;
                }
                filterExpression = (x) =>
                 {
                     if (x.BusinessEntityDefinitionId != businessEntityDefinitionId)
                         return false;
                     return true;

                 };
            }

            return GetOrderedCachedAlertLevels().MapRecords(VRAlertLevelInfoMapper, filterExpression);
        }
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IVRAlertLevelDataManager _dataManager = NotificationDataManagerFactory.GetDataManager<IVRAlertLevelDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreAlertLevelUpdated(ref _updateHandle);
            }
        }

        IOrderedEnumerable<VRAlertLevel> GetOrderedCachedAlertLevels()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetOrderdAlertLevels",
               () =>
               {
                   return GetCachedAlertLevels().Values.OrderBy(x => x.Settings.Weight);
               });
        }

        Dictionary<Guid, VRAlertLevel> GetCachedAlertLevels()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetAlertLevels",
               () =>
               {
                   IVRAlertLevelDataManager dataManager = NotificationDataManagerFactory.GetDataManager<IVRAlertLevelDataManager>();
                   return dataManager.GetAlertLevel().ToDictionary(x => x.VRAlertLevelId, x => x);
               });
        }
        private VRAlertLevelDetail VRAlertLevelDetailMapper(VRAlertLevel alertLevel)
        {
            IBusinessEntityDefinitionManager manager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IBusinessEntityDefinitionManager>();
            VRAlertLevelDetail alertLevelDetail = new VRAlertLevelDetail()
            {
                Entity = alertLevel,
                BusinessEntityDefinitionName = manager.GetBusinessEntityDefinitionName(alertLevel.BusinessEntityDefinitionId)
            };
            return alertLevelDetail;
        }
        private VRAlertLevelInfo VRAlertLevelInfoMapper(VRAlertLevel alertLevel)
        {
            VRAlertLevelInfo alertLevelInfo = new VRAlertLevelInfo()
            {
                VRAlertLevelId = alertLevel.VRAlertLevelId,
                Name = alertLevel.Name
            };
            return alertLevelInfo;
        }
        private class VRAlertLevelLoggableEntity : VRLoggableEntityBase
        {
            public static VRAlertLevelLoggableEntity Instance = new VRAlertLevelLoggableEntity();

            private VRAlertLevelLoggableEntity()
            {

            }

            static VRAlertLevelManager s_vrAlertLevelManager = new VRAlertLevelManager();

            public override string EntityUniqueName
            {
                get { return "VR_Notification_AlertLevel"; }
            }

            public override string ModuleName
            {
                get { return "Notification"; }
            }

            public override string EntityDisplayName
            {
                get { return "Alert Level"; }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "VR_Notification_AlertLevel_ViewHistoryItem"; }
            }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                VRAlertLevel vrAlertLevel = context.Object.CastWithValidate<VRAlertLevel>("context.Object");
                return vrAlertLevel.VRAlertLevelId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                VRAlertLevel vrAlertLevel = context.Object.CastWithValidate<VRAlertLevel>("context.Object");
                return s_vrAlertLevelManager.GetAlertLevelName(vrAlertLevel.VRAlertLevelId);
            }
        }
        #region IBusinessEntityManager

        public override string GetEntityDescription(IBusinessEntityDescriptionContext context)
        {
            return GetAlertLevelName(Guid.Parse(context.EntityId.ToString()));
        }

        public override dynamic GetEntityId(IBusinessEntityIdContext context)
        {
            var vrAlertLevel = context.Entity as VRAlertLevel;
            return vrAlertLevel.VRAlertLevelId;
        }

        public override dynamic GetEntity(IBusinessEntityGetByIdContext context)
        {
            return GetAlertLevel(context.EntityId);
        }

        public override dynamic MapEntityToInfo(IBusinessEntityMapToInfoContext context)
        {
            throw new NotImplementedException();
        }

        public override List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context)
        {
            return GetAllAlertLevels().Select(itm => itm as dynamic).ToList();
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

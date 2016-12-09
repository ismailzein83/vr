using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Notification.BP.Arguments;
using Vanrise.Notification.Data;
using Vanrise.Notification.Entities;
using Vanrise.Common;

namespace Vanrise.Notification.Business
{
    public class VRNotificationManager
    {
        Vanrise.BusinessProcess.Business.BPInstanceManager _bpInstanceManager = new Vanrise.BusinessProcess.Business.BPInstanceManager();
        public CreateVRNotificationOutput CreateNotification(CreateVRNotificationInput input)
        {
            var notification = new VRNotification
            {
                VRNotificationId = Guid.NewGuid(),
                UserId = input.UserId,
                TypeId = input.NotificationTypeId,
                ParentTypes = input.ParentTypes,
                EventKey = input.EventKey,
                Status = VRNotificationStatus.New,
                AlertLevelId = input.AlertLevelId,
                Description = input.Description,
                Data = new VRNotificationData
                {
                    Actions = input.Actions,
                    ClearanceActions = input.ClearanceActions,
                    EventPayload = input.EventPayload,
                    IsAutoClearable = input.IsAutoClearable
                }
            };
            var notificationDataManager = NotificationDataManagerFactory.GetDataManager<IVRNotificationDataManager>();
            notificationDataManager.Insert(notification);
            var executeNotificationProcessInput = new ExecuteNotificationProcessInput
            {
                NotificationId = notification.VRNotificationId,
                UserId = input.UserId
            };
            var createProcessInput = new Vanrise.BusinessProcess.Entities.CreateProcessInput
            {
                InputArguments = executeNotificationProcessInput
            };
            var createProcessOutput = _bpInstanceManager.CreateNewProcess(createProcessInput);
            return new CreateVRNotificationOutput
            {

            };
        }

        public VRNotification GetVRNotificationById(Guid vrNotificationId)
        {
            var allNotifications = GetCachedVRNotifications();
            return allNotifications.GetRecord(vrNotificationId);
        }

        Dictionary<Guid, VRNotification> GetCachedVRNotifications()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetVRNotifications",
               () =>
               {
                   IVRNotificationDataManager dataManager = NotificationDataManagerFactory.GetDataManager<IVRNotificationDataManager>();
                   return dataManager.GetVRNotifications().ToDictionary(x => x.VRNotificationId, x => x);
               });
        }

        public void ClearNotifications(Guid notificationTypeId, VRNotificationParentTypes parentTypes, string eventKey)
        {
            ClearNotificationInput clearNotificationInput = new ClearNotificationInput
            {
                EntityId = eventKey,
                NotificationTypeId = notificationTypeId,
                ParentTypes = parentTypes
            };
            var createProcessInput = new Vanrise.BusinessProcess.Entities.CreateProcessInput
            {
                InputArguments = clearNotificationInput
            };
            _bpInstanceManager.CreateNewProcess(createProcessInput);
        }

        public List<string> GetNotClearedNotificationsEventKeys(Guid notificationTypeId, VRNotificationParentTypes parentTypes, DateTime? notificationCreatedAfter)
        {
            throw new NotImplementedException();
        }

        #region Private Classes

        public class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IVRNotificationDataManager _dataManager = NotificationDataManagerFactory.GetDataManager<IVRNotificationDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreVRNotificationUpdated(ref _updateHandle);
            }
        }

        #endregion


        public void UpdateNotificationStatus(Guid notificationId, VRNotificationStatus vrNotificationStatus)
        {
            IVRNotificationDataManager dataManager = NotificationDataManagerFactory.GetDataManager<IVRNotificationDataManager>();
            dataManager.UpdateNotificationStatus(notificationId, vrNotificationStatus);
        }
    }
}

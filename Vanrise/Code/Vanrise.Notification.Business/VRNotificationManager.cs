using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Notification.BP.Arguments;
using Vanrise.Notification.Data;
using Vanrise.Notification.Entities;
using Vanrise.Common;
using Vanrise.Entities;

namespace Vanrise.Notification.Business
{
    public class VRNotificationManager
    {
        Vanrise.BusinessProcess.Business.BPInstanceManager _bpInstanceManager = new Vanrise.BusinessProcess.Business.BPInstanceManager();
        public CreateVRNotificationOutput CreateNotification(CreateVRNotificationInput input)
        {
            long notificationId = 0;
            var notification = new VRNotification
            {

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
            notificationDataManager.Insert(notification, out notificationId);
            var executeNotificationProcessInput = new ExecuteNotificationProcessInput
            {
                NotificationId = notificationId,
                ProcessTitle = input.Description != null ? input.Description : null,
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

        public VRNotification GetVRNotificationById(long vrNotificationId)
        {
            IVRNotificationDataManager dataManager = NotificationDataManagerFactory.GetDataManager<IVRNotificationDataManager>();
            return dataManager.GetVRNotificationById(vrNotificationId);
        }

        public void ClearNotifications(ClearVRNotificationInput input)
        {
            ClearNotificationInput clearNotificationInput = new ClearNotificationInput
            {
                EventKey = input.EventKey,
                NotificationTypeId = input.NotificationTypeId,
                ParentTypes = input.ParentTypes,
                ProcessTitle = input.Description,
                UserId = input.UserId
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

        public void UpdateNotificationStatus(long notificationId, VRNotificationStatus vrNotificationStatus)
        {
            IVRNotificationDataManager dataManager = NotificationDataManagerFactory.GetDataManager<IVRNotificationDataManager>();
            dataManager.UpdateNotificationStatus(notificationId, vrNotificationStatus);
        }

        public List<VRNotificationDetail> GetUpdatedVRNotifications(VRNotificationUpdateQuery input)
        {
            IVRNotificationDataManager dataManager = NotificationDataManagerFactory.GetDataManager<IVRNotificationDataManager>();
            return dataManager.GetUpdateVRNotifications(input).Select(VRNotificationDetailMapper).ToList();
        }
        public List<VRNotificationDetail> GetBeforeIdVRNotifications(VRNotificationBeforeIdQuery input)
        {
            IVRNotificationDataManager dataManager = NotificationDataManagerFactory.GetDataManager<IVRNotificationDataManager>();
            return dataManager.GetBeforeIdVRNotifications(input).Select(VRNotificationDetailMapper).ToList();
        }
        VRNotificationDetail VRNotificationDetailMapper(VRNotification vrNotification)
        {
            VRNotificationDetail vrNotificationDetail = new VRNotificationDetail()
            {
                Entity = vrNotification
            };
            return vrNotificationDetail;
        }
    }
}

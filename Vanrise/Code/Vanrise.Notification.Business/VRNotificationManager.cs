using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Notification.BP.Arguments;
using Vanrise.Notification.Data;
using Vanrise.Notification.Entities;

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
                Status = VRNotificationStatus.New
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

        public void ClearNotifications(Guid notificationTypeId, VRNotificationParentTypes parentTypes, string eventKey)
        {
            throw new NotImplementedException();
        }

        public List<string> GetNotClearedNotificationsEventKeys(Guid notificationTypeId, VRNotificationParentTypes parentTypes, DateTime? notificationCreatedAfter)
        {
            throw new NotImplementedException();
        }
    }
}

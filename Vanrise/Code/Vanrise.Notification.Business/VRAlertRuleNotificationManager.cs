using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Notification.Entities;

namespace Vanrise.Notification.Business
{
    public class VRAlertRuleNotificationManager
    {
        VRNotificationManager _vrNofiticationManager = new VRNotificationManager();

        public CreateAlertRuleNotificationOutput CreateNotification(CreateAlertRuleNotificationInput notificationInput)
        {
            var notificationParentTypes = BuildNotificationParentTypes(notificationInput.AlertRuleTypeId, notificationInput.AlertRuleId);
            var createVRNotificationInput = new CreateVRNotificationInput
            {
                UserId = notificationInput.UserId,
                NotificationTypeId = notificationInput.NotificationTypeId,
                ParentTypes = notificationParentTypes,
                Description = notificationInput.Description,
                AlertLevelId = notificationInput.AlertLevelId,
                EntityId = notificationInput.EntityId,
                EventKey = notificationInput.EventKey,
                EventPayload = notificationInput.EventPayload,
                Actions = notificationInput.Actions,
                ClearanceActions = notificationInput.ClearanceActions,
                IsAutoClearable = notificationInput.IsAutoClearable
            };
            var createVRNotificationOutput = _vrNofiticationManager.CreateNotification(createVRNotificationInput);
            return new CreateAlertRuleNotificationOutput
            {
                CreateVRNotificationOutput = createVRNotificationOutput
            };
        }

        public void ClearNotifications(ClearAlertRuleNotificationInput notificationInput)
        {
            var notificationParentTypes = BuildNotificationParentTypes(notificationInput.RuleTypeId, notificationInput.AlertRuleId);
            ClearVRNotificationInput clearNotificationInput = new ClearVRNotificationInput
            {
                EventKey = notificationInput.EventKey,
                RollbackEventPayload = notificationInput.RollbackEventPayload,
                NotificationTypeId = notificationInput.NotificationTypeId,
                EntityId = notificationInput.EntityId,
                ParentTypes = notificationParentTypes,
                Description = notificationInput.Description,
                UserId = notificationInput.UserId,
            };
            _vrNofiticationManager.ClearNotifications(clearNotificationInput);
        }

        public Dictionary<string, VRNotification> GetNotClearedAlertRuleNotifications(Guid alertRuleTypeId, Guid notificationTypeId, long? alertRuleId, List<string> eventKeys, DateTime? notificationCreatedAfter)
        {
            var notificationParentTypes = BuildNotificationParentTypes(alertRuleTypeId, alertRuleId);
            return _vrNofiticationManager.GetNotClearedNotifications(notificationTypeId, notificationParentTypes, eventKeys, notificationCreatedAfter);
        }

        public Dictionary<string, VRNotification> GetNotClearedAlertRuleNotifications(Guid alertRuleTypeId, Guid notificationTypeId)
        {
            return GetNotClearedAlertRuleNotifications(alertRuleTypeId, notificationTypeId, null, null, null);
        }

        #region Private Methods

        private VRNotificationParentTypes BuildNotificationParentTypes(Guid alertRuleTypeId, long? alertRuleId)
        {
            VRNotificationParentTypes parentTypes = new VRNotificationParentTypes
            {
                ParentType1 = alertRuleTypeId.ToString()
            };
            if (alertRuleId.HasValue)
                parentTypes.ParentType2 = alertRuleId.Value.ToString();
            return parentTypes;
        }

        #endregion
    }
}

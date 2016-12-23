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
            Guid notificationTypeId = GetNotificationTypeId(notificationInput.AlertRuleTypeId);
            var notificationParentTypes = BuildNotificationParentTypes(notificationInput.AlertRuleTypeId, notificationInput.AlertRuleId);
            var createVRNotificationInput = new CreateVRNotificationInput
            {
                UserId = notificationInput.UserId,
                NotificationTypeId = notificationTypeId,
                ParentTypes = notificationParentTypes,
                Description = notificationInput.Description,
                AlertLevelId = notificationInput.AlertLevelId,
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
            Guid notificationTypeId = GetNotificationTypeId(notificationInput.RuleTypeId);
            var notificationParentTypes = BuildNotificationParentTypes(notificationInput.RuleTypeId, notificationInput.AlertRuleId);
            ClearVRNotificationInput clearNotificationInput = new ClearVRNotificationInput
            {
                EventKey = notificationInput.EventKey,
                NotificationTypeId = notificationTypeId,
                ParentTypes = notificationParentTypes,
                UserId = notificationInput.UserId
            };
            _vrNofiticationManager.ClearNotifications(clearNotificationInput);
        }

        public List<string> GetNotClearedNotificationsEventKeys(Guid alertRuleTypeId, long? alertRuleId, DateTime? notificationCreatedAfter)
        {
            Guid notificationTypeId = GetNotificationTypeId(alertRuleTypeId);
            var notificationParentTypes = BuildNotificationParentTypes(alertRuleTypeId, alertRuleId);
            return _vrNofiticationManager.GetNotClearedNotificationsEventKeys(notificationTypeId, notificationParentTypes, notificationCreatedAfter);
        }


        public List<string> GetNotClearedNotificationsEventKeys(Guid alertRuleTypeId)
        {
            return GetNotClearedNotificationsEventKeys(alertRuleTypeId, null, null);
        }

        #region Private Methods

        static VRAlertRuleTypeManager s_alertRuleTypeManager = new VRAlertRuleTypeManager();

        private Guid GetNotificationTypeId(Guid alertRuleTypeId)
        {
            return s_alertRuleTypeManager.GetVRAlertRuleTypeSettings<VRAlertRuleTypeSettings>(alertRuleTypeId).NotificationTypeId;
        }

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

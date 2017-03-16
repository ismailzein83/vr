using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Notification.Entities;

namespace Vanrise.GenericData.Notification
{
    public class DataRecordNotificationTypeSettings : VRNotificationTypeExtendedSettings
    {
        public override Guid ConfigId { get { return new Guid("E64C51A2-08E0-4B7D-96F0-9FF1848A72FA"); } }

        public override string SearchRuntimeEditor { get { return "vr-genericdata-notificationtypesettings-searcheditor"; } }

        public override string BodyRuntimeEditor { get { return "vr-genericdata-notificationtypesettings-bodyeditor"; } }

        public Guid DataRecordTypeId { get; set; }

        public List<NotificationGridColumnDefinition> GridColumnDefinitions { get; set; }

        public override VRNotificationDetail GetVRNotificationDetailMapper(IGetVRNotificationDetailMapperContext context)
        {
            VRNotification vrNotification = context.VRNotification;
            if (vrNotification == null)
                throw new NullReferenceException("vrNotification");

            VRNotificationData vrNotificationData = context.VRNotification.Data;
            if (vrNotificationData == null)
                throw new NullReferenceException("vrNotificationData");

            DataRecordAlertRuleActionEventPayload eventPayload = vrNotificationData.EventPayload as DataRecordAlertRuleActionEventPayload;
            if (eventPayload == null)
                throw new NullReferenceException("dataRecordAlertRuleActionEventPayload");

            AlertRuleActionEventPayloadDetail alertRuleActionEventPayloadDetail = new AlertRuleActionEventPayloadDetail()
            {
                Entity = context.VRNotification,
                FieldValues = eventPayload.OutputRecords
            };
            return alertRuleActionEventPayloadDetail;
        }
    }

    public class NotificationGridColumnDefinition
    {
        public string FieldName { get; set; }

        public string Header { get; set; }
    }

    public class AlertRuleActionEventPayloadDetail : VRNotificationDetail
    {
        public Dictionary<string, dynamic> FieldValues { get; set; }
    }
}
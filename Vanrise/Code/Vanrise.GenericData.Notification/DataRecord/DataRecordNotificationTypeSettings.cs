using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Notification.Entities;

namespace Vanrise.GenericData.Notification 
{
    public class DataRecordNotificationTypeSettings : VRNotificationTypeExtendedSettings
    {
        public override Guid ConfigId { get { return new Guid("E64C51A2-08E0-4B7D-96F0-9FF1848A72FA"); } }

        public override string SearchRuntimeEditor { get { return "vr-genericdata-datarecordnotificationtypesettings-searcheditor"; } }

        public override string BodyRuntimeEditor { get { return "vr-genericdata-datarecordnotificationtypesettings-bodyeditor"; } }

        public Guid DataRecordTypeId { get; set; }

        public List<NotificationGridColumnDefinition> GridColumnDefinitions { get; set; }

        public override VRNotificationDetail MapToNotificationDetail(IMapToNotificationDetailContext context)
        {
            Dictionary<string, NotificationFieldValue> fieldValues = new Dictionary<string, NotificationFieldValue>();
            DataRecordTypeManager dataRecordTypeManager = new DataRecordTypeManager();

            VRNotification vrNotification = context.VRNotification;
            if (vrNotification == null)
                throw new NullReferenceException("vrNotification");

            VRNotificationData vrNotificationData = context.VRNotification.Data;
            if (vrNotificationData == null)
                throw new NullReferenceException("vrNotificationData");

            DataRecordAlertRuleActionEventPayload eventPayload = vrNotificationData.EventPayload as DataRecordAlertRuleActionEventPayload;
            if (eventPayload == null)
                throw new NullReferenceException("dataRecordAlertRuleActionEventPayload");

            foreach(var outputRecord in eventPayload.OutputRecords) 
            {
                string fieldName = outputRecord.Key;
                dynamic fieldValue = outputRecord.Value;

                DataRecordField dataRecordField = dataRecordTypeManager.GetDataRecordField(this.DataRecordTypeId, fieldName);
                if (dataRecordField == null)
                    throw new NullReferenceException(string.Format("dataRecordField of DataRecordTypeId: {0} and FieldName: {1}", this.DataRecordTypeId, fieldName));

                NotificationFieldValue notificationFieldValue = new NotificationFieldValue();
                notificationFieldValue.Value = fieldValue;
                notificationFieldValue.Description = dataRecordField.Type.GetDescription(fieldValue);

                fieldValues.Add(fieldName, notificationFieldValue);
            }

            AlertRuleActionEventPayloadDetail alertRuleActionEventPayloadDetail = new AlertRuleActionEventPayloadDetail()
            {
                Entity = context.VRNotification,
                FieldValues = fieldValues
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
        public Dictionary<string, NotificationFieldValue> FieldValues { get; set; }
    }

    public class NotificationFieldValue
    {
        public Object Value { get; set; }

        public string Description { get; set; }
    }
}
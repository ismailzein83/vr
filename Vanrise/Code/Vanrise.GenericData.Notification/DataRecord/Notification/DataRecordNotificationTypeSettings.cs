using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Notification.Entities;
using Vanrise.Notification.Business;

namespace Vanrise.GenericData.Notification
{
    public class DataRecordNotificationTypeSettings : VRNotificationTypeExtendedSettings
    {
        RecordFilterManager recordFilterManager = new Vanrise.GenericData.Business.RecordFilterManager();

        public override Guid ConfigId { get { return new Guid("E64C51A2-08E0-4B7D-96F0-9FF1848A72FA"); } }

        public override string SearchRuntimeEditor { get { return "vr-genericdata-datarecordnotificationtypesettings-searcheditor"; } }

        public override string BodyRuntimeEditor { get { return "vr-genericdata-datarecordnotificationtypesettings-bodyeditor"; } }

        public Guid DataRecordTypeId { get; set; }

        public List<NotificationGridColumnDefinition> GridColumnDefinitions { get; set; }


        public override bool IsVRNotificationMatched(IVRNotificationTypeIsMatchedContext context)
        {
            if (context.ExtendedQuery == null)
                return true;

            var extendedQuery = context.ExtendedQuery as DataRecordNotificationExtendedQuery;
            if (extendedQuery == null)
                return false;

            if (extendedQuery.FilterGroup == null)
                return true;

            if (context.VRNotification == null || context.VRNotification.EventPayload == null)
                return false;

            DataRecordAlertRuleActionEventPayload dataRecordAlertRuleActionEventPayload = context.VRNotification.EventPayload as DataRecordAlertRuleActionEventPayload;
            if (dataRecordAlertRuleActionEventPayload == null)
                return false;

            return recordFilterManager.IsFilterGroupMatch(extendedQuery.FilterGroup, new DataRecordDictFilterGenericFieldMatchContext(dataRecordAlertRuleActionEventPayload.OutputRecords, this.DataRecordTypeId));
        }

        public override VRNotificationDetailEventPayload GetNotificationDetailEventPayload(IVRNotificationTypeGetNotificationEventPayloadContext context)
        {
            Dictionary<string, DataRecordFieldValue> fieldValues = new Dictionary<string, DataRecordFieldValue>();
            DataRecordTypeManager dataRecordTypeManager = new DataRecordTypeManager();

            List<string> definedGridColumnFieldNames = this.GridColumnDefinitions != null ? this.GridColumnDefinitions.Select(itm => itm.FieldName).ToList() : null;
            if (definedGridColumnFieldNames == null || definedGridColumnFieldNames.Count == 0)
                throw new Exception("At least a Grid Column should be Defined!!");

            DataRecordAlertRuleActionEventPayload eventPayload = new VRNotificationManager().GetVRNotificationEventPayload<DataRecordAlertRuleActionEventPayload>(context.VRNotification);

            foreach (var outputRecord in eventPayload.OutputRecords)
            {
                string fieldName = outputRecord.Key;
                dynamic fieldValue = outputRecord.Value;

                if (!definedGridColumnFieldNames.Contains(fieldName))
                    continue;

                DataRecordField dataRecordField = dataRecordTypeManager.GetDataRecordField(this.DataRecordTypeId, fieldName);
                if (dataRecordField == null)
                    throw new NullReferenceException(string.Format("dataRecordField of DataRecordTypeId: {0} and FieldName: {1}", this.DataRecordTypeId, fieldName));

                DataRecordFieldValue dataRecordFieldValue = new DataRecordFieldValue();
                dataRecordFieldValue.Value = fieldValue;
                dataRecordFieldValue.Description = dataRecordField.Type.GetDescription(fieldValue);

                fieldValues.Add(fieldName, dataRecordFieldValue);
            }

            return new DataRecordNotificationDetailEventPayload() { FieldValues = fieldValues };
        }
    }

    public class NotificationGridColumnDefinition
    {
        public string FieldName { get; set; }

        public string Header { get; set; }
    }

    public class DataRecordNotificationDetailEventPayload : VRNotificationDetailEventPayload
    {
        public Dictionary<string, DataRecordFieldValue> FieldValues { get; set; }
    }
}
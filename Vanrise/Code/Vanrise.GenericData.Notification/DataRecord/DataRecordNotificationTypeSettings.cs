using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Notification.Entities;

namespace Vanrise.GenericData.Notification
{
    public class DataRecordNotificationTypeSettings : VRNotificationTypeExtendedSettings
    {
        #region Ctor/Properties

        RecordFilterManager recordFilterManager = new Vanrise.GenericData.Business.RecordFilterManager();

        public override Guid ConfigId { get { return new Guid("E64C51A2-08E0-4B7D-96F0-9FF1848A72FA"); } }

        public override string SearchRuntimeEditor { get { return "vr-genericdata-datarecordnotificationtypesettings-searcheditor"; } }

        public override string BodyRuntimeEditor { get { return "vr-genericdata-datarecordnotificationtypesettings-bodyeditor"; } }

        public Guid DataRecordTypeId { get; set; }

        public List<NotificationGridColumnDefinition> GridColumnDefinitions { get; set; }

        #endregion

        #region Public Methods

        public override bool IsVRNotificationMatched(IVRNotificationTypeIsMatchedContext context)
        {
            if (context.ExtendedQuery == null)
                return true;

            var extendedQuery = context.ExtendedQuery as DataRecordNotificationExtendedQuery;
            if (extendedQuery == null)
                return true;

            if (extendedQuery.FilterGroup == null)
                return true;
            
            return recordFilterManager.IsFilterGroupMatch(extendedQuery.FilterGroup, new DataRecordNotificationTypeRecordFilterGenericFieldMatchContext(this.DataRecordTypeId, context.VRNotification));
        }

        public override VRNotificationDetail MapToNotificationDetail(IVRNotificationTypeMapToDetailContext context)
        {
            Dictionary<string, DataRecordFieldValue> fieldValues = new Dictionary<string, DataRecordFieldValue>();
            DataRecordTypeManager dataRecordTypeManager = new DataRecordTypeManager();

            List<string> definedGridColumnFieldNames = this.GridColumnDefinitions != null ? this.GridColumnDefinitions.Select(itm => itm.FieldName).ToList() : null;
            if (definedGridColumnFieldNames == null || definedGridColumnFieldNames.Count == 0)
                throw new Exception("At least a Grid Column should be Defined!!");

            DataRecordAlertRuleActionEventPayload eventPayload = GetDataRecordAlertRuleActionEventPayload(context.VRNotification);

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

            AlertRuleActionEventPayloadDetail alertRuleActionEventPayloadDetail = new AlertRuleActionEventPayloadDetail()
            {
                Entity = context.VRNotification,
                FieldValues = fieldValues
            };
            return alertRuleActionEventPayloadDetail;
        }

        #endregion

        #region Private Methods

        private static DataRecordAlertRuleActionEventPayload GetDataRecordAlertRuleActionEventPayload(VRNotification vrNotification)
        {
            if (vrNotification == null)
                throw new NullReferenceException("vrNotification");

            VRNotificationData vrNotificationData = vrNotification.Data;
            if (vrNotificationData == null)
                throw new NullReferenceException("vrNotificationData");

            DataRecordAlertRuleActionEventPayload eventPayload = vrNotificationData.EventPayload as DataRecordAlertRuleActionEventPayload;
            if (eventPayload == null)
                throw new NullReferenceException("dataRecordAlertRuleActionEventPayload");

            return eventPayload;
        }

        #endregion

        #region Private Classes

        private class DataRecordNotificationTypeRecordFilterGenericFieldMatchContext : IRecordFilterGenericFieldMatchContext
        {
            Guid _dataRecordType;
            DataRecordAlertRuleActionEventPayload _eventPayload;
            static DataRecordTypeManager dataRecordTypeManager = new DataRecordTypeManager();

            public DataRecordNotificationTypeRecordFilterGenericFieldMatchContext(Guid dataRecordType, VRNotification _vrNotification)
            {
                _dataRecordType = dataRecordType;
                _eventPayload = GetDataRecordAlertRuleActionEventPayload(_vrNotification);
            }

            public object GetFieldValue(string fieldName, out DataRecordFieldType fieldType)
            {
                DataRecordField dataRecordField = dataRecordTypeManager.GetDataRecordField(_dataRecordType, fieldName);
                if (dataRecordField == null)
                    throw new NullReferenceException(string.Format("dataRecordField of DataRecordTypeId: {0} and FieldName: {1}", _dataRecordType, fieldName));

                Dictionary<string, dynamic> outputRecords = _eventPayload.OutputRecords;
                if (outputRecords == null)
                    throw new NullReferenceException("dataRecordAlertRuleActionEventPayload.OutputRecords");

                object fieldValue = null;
                if (!outputRecords.TryGetValue(fieldName, out fieldValue))
                    throw new Exception(string.Format("dataRecordAlertRuleActionEventPayload.OutputRecords does not contain fieldName: {0}", fieldName));

                fieldType = dataRecordField.Type;
                return fieldValue;
            }
        }

        #endregion
    }

    public class NotificationGridColumnDefinition
    {
        public string FieldName { get; set; }

        public string Header { get; set; }
    }

    public class AlertRuleActionEventPayloadDetail : VRNotificationDetail
    {
        public Dictionary<string, DataRecordFieldValue> FieldValues { get; set; }
    }
}
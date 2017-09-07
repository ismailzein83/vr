using System;
using System.Collections.Generic;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Notification.Business;

namespace Vanrise.GenericData.Notification
{
    public class DataRecordNotificationTypeSettingsManager
    {
        public List<DataRecordGridColumnAttribute> GetNotificationGridColumnAttributes(Guid notificationTypeId)
        {
            List<DataRecordGridColumnAttribute> results = new List<DataRecordGridColumnAttribute>();
            DataRecordTypeManager dataRecordTypeManager = new DataRecordTypeManager();

            var vrNotificationTypeExtendedSettings = new VRNotificationTypeManager().GetVRNotificationTypeExtendedSettings<DataRecordNotificationTypeSettings>(notificationTypeId);
            Guid dataRecordTypeId = vrNotificationTypeExtendedSettings.DataRecordTypeId;

            foreach (NotificationGridColumnDefinition gridColumnDefinition in vrNotificationTypeExtendedSettings.GridColumnDefinitions)
            {
                DataRecordField dataRecordField = dataRecordTypeManager.GetDataRecordField(dataRecordTypeId, gridColumnDefinition.FieldName);
                if (dataRecordField == null)
                    throw new NullReferenceException(string.Format("dataRecordField of DataRecordTypeId: {0} and FieldName: {1}", dataRecordTypeId, gridColumnDefinition.FieldName));

                FieldTypeGetGridColumnAttributeContext context = new FieldTypeGetGridColumnAttributeContext();
                context.ValueFieldPath = "VRNotificationEventPayload.FieldValues." + gridColumnDefinition.FieldName + ".Value";
                context.DescriptionFieldPath = "VRNotificationEventPayload.FieldValues." + gridColumnDefinition.FieldName + ".Description";

                DataRecordGridColumnAttribute attribute = new DataRecordGridColumnAttribute()
                {
                    Attribute = dataRecordField.Type.GetGridColumnAttribute(context),
                    Name = gridColumnDefinition.FieldName,
                    DetailViewerEditor = dataRecordField.Type.DetailViewerEditor
                };
                attribute.Attribute.HeaderText = gridColumnDefinition.Header;

                results.Add(attribute);
            }

            return results;
        }
    }
}
